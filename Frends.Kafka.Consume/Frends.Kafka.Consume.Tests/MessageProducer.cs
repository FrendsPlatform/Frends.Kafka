using Avro;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Frends.Kafka.Consume.Definitions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable CS8600,CS8603,CS8601,CS8604, CS8602
namespace Frends.Kafka.Consume.Tests
{
    internal class MessageProducer
    {
        internal static async Task ProduceBasic(Input input, Options options, Socket socket, Sasl sasl, Ssl ssl, string? msgKey, string? msg, CancellationToken cancellationToken)
        {
            try
            {
                using var producer = new ProducerBuilder<string, string>(GetProducerConfig(input, options, socket, sasl, ssl)).Build();

                TopicPartition topicPartition;
                if (input.Partition >= 0)
                    topicPartition = new TopicPartition(input.Topic, new Partition(input.Partition));
                else
                    topicPartition = new TopicPartition(input.Topic, new Partition());

                Message<string, string> message = new()
                {
                    Key = msgKey ?? null,
                    Value = msg
                };

                var result = await producer.ProduceAsync(topicPartition, message, cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static async Task ProduceAvro(Input input, Options options, Socket socket, Sasl sasl, Ssl ssl, string? recordJson, string? schema, string? msgKey, SchemaRegistry schemaRegistry, CancellationToken cancellationToken)
        {
            var schemaRegistryConfig = new SchemaRegistryConfig()
            {
                Url = AssignIfNotNullOrEmpty(schemaRegistry.SchemaRegistryUrl, string.Empty),
                BasicAuthCredentialsSource = GetBasicAuthCredentialsSource(schemaRegistry.BasicAuthCredentialsSource),
                BasicAuthUserInfo = AssignIfNotNullOrEmpty(schemaRegistry.BasicAuthUserInfo, string.Empty),
                RequestTimeoutMs = schemaRegistry.RequestTimeoutMs,
                MaxCachedSchemas = schemaRegistry.MaxCachedSchemas,
            };

            if (!string.IsNullOrEmpty(schemaRegistry.SslCaLocation))
            {
                schemaRegistryConfig.SslCaLocation = AssignIfNotNullOrEmpty(schemaRegistry.SslCaLocation, string.Empty);
                schemaRegistryConfig.SslKeystorePassword = AssignIfNotNullOrEmpty(schemaRegistry.SslKeystorePassword, string.Empty);
                schemaRegistryConfig.SslKeystoreLocation = AssignIfNotNullOrEmpty(schemaRegistry.SslKeystoreLocation, string.Empty);
            }

            TopicPartition topicPartition = null;
            if (input.Partition >= 0)
                topicPartition = new TopicPartition(input.Topic, new Partition(input.Partition));

            using var cachedSchemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
            using var producer = new ProducerBuilder<string, GenericRecord>(GetProducerConfig(input, options, socket, sasl, ssl))
                .SetValueSerializer(new AvroSerializer<GenericRecord>(cachedSchemaRegistryClient))
                .Build();

            var avroSchema = (RecordSchema)Avro.Schema.Parse(schema);

            JObject recordsJObject = JObject.Parse(recordJson);

            var records = new GenericRecord(avroSchema);
            AddFields(avroSchema, recordsJObject, records);

            if (topicPartition != null)
                await producer.ProduceAsync(topicPartition, new Message<string, GenericRecord> { Key = msgKey, Value = records }, cancellationToken);
            else
                await producer.ProduceAsync(input.Topic, new Message<string, GenericRecord> { Key = msgKey, Value = records }, cancellationToken);
        }

        private static void AddFields(RecordSchema schema, JObject jObject, GenericRecord record)
        {
            foreach (var field in schema.Fields)
            {
                if (jObject.TryGetValue(field.Name, out JToken token))
                {
                    // Field is present in JObject
                    switch (field.Schema.Tag)
                    {
                        case Avro.Schema.Type.Enumeration:
                            var enumSchema = (EnumSchema)field.Schema;
                            record.Add(field.Name, new GenericEnum(enumSchema, token.ToObject<string>()));
                            break;
                        case Avro.Schema.Type.Fixed:
                            var fixedSchema = (FixedSchema)field.Schema;
                            var bytes = Convert.FromBase64String(token.ToObject<string>());
                            record.Add(field.Name, new GenericFixed(fixedSchema, bytes));
                            break;
                        case Avro.Schema.Type.Array:
                            var array = token.ToObject<string[]>();
                            record.Add(field.Name, array);
                            break;
                        case Avro.Schema.Type.Null:
                            record.Add(field.Name, null);
                            break;
                        case Avro.Schema.Type.Boolean:
                            record.Add(field.Name, token.ToObject<bool>());
                            break;
                        case Avro.Schema.Type.Int:
                            record.Add(field.Name, token.ToObject<int>());
                            break;
                        case Avro.Schema.Type.Long:
                            record.Add(field.Name, token.ToObject<long>());
                            break;
                        case Avro.Schema.Type.Float:
                            record.Add(field.Name, token.ToObject<float>());
                            break;
                        case Avro.Schema.Type.Double:
                            record.Add(field.Name, token.ToObject<double>(new JsonSerializer { Culture = CultureInfo.InvariantCulture }));
                            break;
                        case Avro.Schema.Type.Bytes:
                            record.Add(field.Name, Encoding.UTF8.GetBytes(token.ToObject<string>()));
                            break;
                        case Avro.Schema.Type.String:
                            record.Add(field.Name, token.ToObject<string>());
                            break;
                        case Avro.Schema.Type.Record:
                            var recordSchema = (RecordSchema)field.Schema;
                            var nestedRecord = new GenericRecord(recordSchema);
                            AddFields(recordSchema, (JObject)token, nestedRecord);
                            record.Add(field.Name, nestedRecord);
                            break;
                        case Avro.Schema.Type.Map:
                            var mapObject = token.ToObject<Dictionary<string, JToken>>();
                            var resultMap = new Dictionary<string, object>();
                            foreach (var kvp in mapObject)
                                resultMap[kvp.Key] = ConvertTokenToAvroType(kvp.Value, kvp.Value.Type);
                            record.Add(field.Name, resultMap);
                            break;
                        case Avro.Schema.Type.Union:
                            var unionSchema = (UnionSchema)field.Schema;
                            if (unionSchema.Schemas[1].Tag == Avro.Schema.Type.Null && token.Type == JTokenType.Null)
                                record.Add(field.Name, null);
                            else if (unionSchema.Schemas[1].Tag == Avro.Schema.Type.String && token.Type == JTokenType.String)
                                record.Add(field.Name, token.ToObject<string>());
                            else
                                throw new InvalidOperationException($"Unsupported union type: {unionSchema.Schemas[1].Tag}");
                            break;
                        case Avro.Schema.Type.Error:
                            throw new InvalidOperationException("Error type not supported");
                        case Avro.Schema.Type.Logical:
                            throw new InvalidOperationException("Logical type not supported");
                        default:
                            throw new InvalidOperationException($"Unsupported Avro type: {field.Schema.Tag}");
                    }
                }
                else
                {
                    // Field is not present in JObject
                    if (field.Schema is UnionSchema unionSchema)
                    {
                        // Field is optional
                        if (field.DefaultValue != null)
                        {
                            // Set field to default value
                            switch (unionSchema.Schemas[1].Tag)
                            {
                                case Avro.Schema.Type.Record:
                                    var nestedRecord = new GenericRecord((RecordSchema)unionSchema.Schemas[1]);
                                    AddFields((RecordSchema)unionSchema.Schemas[1], JObject.Parse(field.DefaultValue.ToString()), nestedRecord);
                                    record.Add(field.Name, nestedRecord);
                                    break;
                                default:
                                    record.Add(field.Name, field.DefaultValue);
                                    break;
                            }
                        }
                        else
                        {
                            // Set field to null
                            record.Add(field.Name, null);
                        }
                    }
                    else
                    {
                        // Field is required but not present in JObject
                        throw new InvalidOperationException($"Missing required field: {field.Name}");
                    }
                }
            }
        }

        private static object ConvertTokenToAvroType(JToken token, JTokenType type)
        {

            return type switch
            {
                JTokenType.Null => null,
                JTokenType.Boolean => token.ToObject<bool>(),
                JTokenType.Integer => token.ToObject<int>(),
                JTokenType.Float => token.ToObject<double>(),
                JTokenType.String => token.ToObject<string>(),
                JTokenType.Bytes => Encoding.UTF8.GetBytes(token.ToObject<string>()),
                _ => throw new InvalidOperationException($"Unsupported token type: {type}"),
            };
        }

        private static ProducerConfig GetProducerConfig(Input input, Options options, Socket socket, Sasl sasl, Ssl ssl)
        {
            ProducerConfig config = new()
            {
                Acks = GetAcks(options.Acks),
                BootstrapServers = AssignIfNotNullOrEmpty(input.Host, string.Empty),
                MaxInFlight = options.MaxInFlight,
                MessageMaxBytes = options.MessageMaxBytes,
                SecurityProtocol = GetSecurityProtocol(input.SecurityProtocol),
                SocketTimeoutMs = socket.SocketTimeoutMs,
                SocketConnectionSetupTimeoutMs = socket.SocketConnectionSetupTimeoutMs,
                SocketKeepaliveEnable = socket.SocketKeepaliveEnable,
                SocketMaxFails = socket.SocketMaxFails,
                SocketNagleDisable = socket.SocketNagleDisable,
                SocketReceiveBufferBytes = socket.SocketReceiveBufferBytes,
            };

            // This IF statement is required because the setter does not like nulls or empty strings in cases where we do not want to assign anything.
            if (!string.IsNullOrEmpty(options.Debug))
                config.Debug = options.Debug;

            if (sasl.UseSasl)
            {
                config.SaslMechanism = GetSaslMechanism(sasl.SaslMechanism);
                config.SaslUsername = AssignIfNotNullOrEmpty(sasl.SaslUsername, string.Empty);
                config.SaslPassword = AssignIfNotNullOrEmpty(sasl.SaslPassword, string.Empty);
                config.SaslOauthbearerMethod = GetSaslOauthbearerMethod(sasl.SaslOauthbearerMethod);
                config.SaslOauthbearerClientId = AssignIfNotNullOrEmpty(sasl.SaslOauthbearerClientId, string.Empty);
                config.SaslOauthbearerClientSecret = AssignIfNotNullOrEmpty(sasl.SaslOauthbearerClientSecret, string.Empty);
                config.SaslOauthbearerTokenEndpointUrl = AssignIfNotNullOrEmpty(sasl.SaslOauthbearerTokenEndpointUrl, string.Empty);
                config.SaslOauthbearerConfig = AssignIfNotNullOrEmpty(sasl.SaslOauthbearerConfig, string.Empty);
                config.SaslOauthbearerExtensions = AssignIfNotNullOrEmpty(sasl.SaslOauthbearerExtensions, string.Empty);
                config.SaslOauthbearerScope = AssignIfNotNullOrEmpty(sasl.SaslOauthbearerScope, string.Empty);
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                config.SaslKerberosKeytab = AssignIfNotNullOrEmpty(sasl.SaslKerberosKeytab, string.Empty);
                config.SaslKerberosMinTimeBeforeRelogin = sasl.SaslKerberosMinTimeBeforeRelogin;
                config.SaslKerberosPrincipal = AssignIfNotNullOrEmpty(sasl.SaslKerberosPrincipal, "kafkaclient");
                config.SaslKerberosServiceName = AssignIfNotNullOrEmpty(sasl.SaslKerberosServiceName, "kafka");
            }

            if (ssl.UseSsl)
            {
                config.SslEndpointIdentificationAlgorithm = GetSslEndpointIdentificationAlgorithm(ssl.SslEndpointIdentificationAlgorithm);
                config.EnableSslCertificateVerification = ssl.EnableSslCertificateVerification;
                config.SslCertificateLocation = AssignIfNotNullOrEmpty(ssl.SslCertificateLocation, string.Empty);
                config.SslCertificatePem = AssignIfNotNullOrEmpty(ssl.SslCertificatePem, string.Empty);
                config.SslCaCertificateStores = AssignIfNotNullOrEmpty(ssl.SslCaCertificateStores, "Root");
                config.SslCaLocation = AssignIfNotNullOrEmpty(ssl.SslCaLocation, string.Empty);
                config.SslCaPem = AssignIfNotNullOrEmpty(ssl.SslCaPem, string.Empty);
                config.SslKeyLocation = AssignIfNotNullOrEmpty(ssl.SslKeyLocation, string.Empty);
                config.SslKeyPassword = AssignIfNotNullOrEmpty(ssl.SslKeyPassword, string.Empty);
                config.SslKeyPem = AssignIfNotNullOrEmpty(ssl.SslKeyPem, string.Empty);
                config.SslKeystoreLocation = AssignIfNotNullOrEmpty(ssl.SslKeystoreLocation, string.Empty);
                config.SslKeystorePassword = AssignIfNotNullOrEmpty(ssl.SslKeystorePassword, string.Empty);
                config.SslEngineLocation = AssignIfNotNullOrEmpty(ssl.SslEngineLocation, string.Empty);
                config.SslCipherSuites = AssignIfNotNullOrEmpty(ssl.SslCipherSuites, string.Empty);
                config.SslCrlLocation = AssignIfNotNullOrEmpty(ssl.SslCrlLocation, string.Empty);
                config.SslCurvesList = AssignIfNotNullOrEmpty(ssl.SslCurvesList, string.Empty);
                config.SslSigalgsList = AssignIfNotNullOrEmpty(ssl.SslSigalgsList, string.Empty);
            }

            return config;
        }

        private static string AssignIfNotNullOrEmpty(string source, string defaultValue)
        {
            return !string.IsNullOrEmpty(source) ? source : defaultValue;
        }

        private static AuthCredentialsSource GetBasicAuthCredentialsSource(AuthCredentialsSources authCredentialsSources)
        {
            return authCredentialsSources switch
            {
                AuthCredentialsSources.UserInfo => AuthCredentialsSource.UserInfo,
                AuthCredentialsSources.SaslInherit => AuthCredentialsSource.SaslInherit,
                _ => throw new ArgumentException($"GetBasicAuthCredentialsSource exception: Value '{authCredentialsSources}' not supported."),
            };
        }

        private static SecurityProtocol GetSecurityProtocol(SecurityProtocols securityProtocols)
        {
            return securityProtocols switch
            {
                SecurityProtocols.Plaintext => SecurityProtocol.Plaintext,
                SecurityProtocols.Ssl => SecurityProtocol.Ssl,
                SecurityProtocols.SaslPlaintext => SecurityProtocol.SaslPlaintext,
                SecurityProtocols.SaslSsl => SecurityProtocol.SaslSsl,
                _ => throw new ArgumentException($"GetSecurityProtocol exception: Value '{securityProtocols}' not supported."),
            };
        }

        private static SslEndpointIdentificationAlgorithm GetSslEndpointIdentificationAlgorithm(SslEndpointIdentificationAlgorithms sslEndpointIdentificationAlgorithms)
        {
            return sslEndpointIdentificationAlgorithms switch
            {
                SslEndpointIdentificationAlgorithms.None => SslEndpointIdentificationAlgorithm.None,
                SslEndpointIdentificationAlgorithms.Https => SslEndpointIdentificationAlgorithm.Https,
                _ => throw new ArgumentException($"GetSslEndpointIdentificationAlgorithm exception: Value '{sslEndpointIdentificationAlgorithms}' not supported."),
            };
        }

        private static Acks GetAcks(Ack ack)
        {
            return ack switch
            {
                Ack.None => Acks.None,
                Ack.Leader => Acks.Leader,
                Ack.All => Acks.All,
                _ => throw new ArgumentException($"GetAcks exception: Value {ack} not supported."),
            };
        }

        private static SaslOauthbearerMethod GetSaslOauthbearerMethod(SaslOauthbearerMethods saslOauthbearerMethods)
        {
            return saslOauthbearerMethods switch
            {
                SaslOauthbearerMethods.Default => SaslOauthbearerMethod.Default,
                SaslOauthbearerMethods.Oidc => SaslOauthbearerMethod.Oidc,
                _ => throw new ArgumentException($"GetSaslOauthbearerMethod exception: Value '{saslOauthbearerMethods}' not supported."),
            };
        }

        private static SaslMechanism GetSaslMechanism(SaslMechanisms saslMechanisms)
        {
            return saslMechanisms switch
            {
                SaslMechanisms.Gssapi => SaslMechanism.Gssapi,
                SaslMechanisms.Plain => SaslMechanism.Plain,
                SaslMechanisms.ScramSha256 => SaslMechanism.ScramSha256,
                SaslMechanisms.ScramSha512 => SaslMechanism.ScramSha512,
                SaslMechanisms.OAuthBearer => SaslMechanism.OAuthBearer,
                _ => throw new ArgumentException($"GetSaslMechanism exception: Value '{saslMechanisms}' not supported."),
            };
        }
    }
}
