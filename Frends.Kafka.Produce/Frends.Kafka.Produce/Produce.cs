﻿using Confluent.Kafka;
using Frends.Kafka.Produce.Definitions;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Kafka.Produce;

/// <summary>
/// Kafka Task
/// </summary>
public class Kafka
{
    /// <summary>
    /// Kafka produce operation.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Kafka.Produce)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="options">Optional parameters.</param>
    /// <param name="socket">Socket parametes.</param>
    /// <param name="sasl">SASL parameters.</param>
    /// <param name="ssl">SSL parameters.</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this Task.</param>
    /// <returns>Object { bool Success, string Status, string Timestamp }</returns>
    public static async Task<Result> Produce([PropertyTab] Input input, [PropertyTab] Options options, [PropertyTab] Socket socket, [PropertyTab] Sasl sasl, [PropertyTab] Ssl ssl, CancellationToken cancellationToken)
    {
        try
        {
            var config = Configurations(input, options, socket, sasl, ssl);
            using var producer = new ProducerBuilder<string, string>(config).Build();

            TopicPartition topicPartition;
            if (input.Partition >= 0)
                topicPartition = new TopicPartition(input.Topic, new Partition(input.Partition));
            else
                topicPartition = new TopicPartition(input.Topic, new Partition());

            Message<string, string> message = new()
            {
                Key = input.Key ?? null,
                Value = JsonSerializer.Serialize(input.Message)
            };

            var result = await producer.ProduceAsync(topicPartition, message, cancellationToken);
            return new Result(true, result.Status.ToString(), result.Timestamp.UtcDateTime.ToString());
        }
        catch (KafkaException ke)
        {
            throw new Exception($"Produce KafkaException: {ke}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Produce Exception: {ex}");
        }
    }

    private static ProducerConfig Configurations(Input input, Options options, Socket socket, Sasl sasl, Ssl ssl)
    {
        ProducerConfig config = new()
        {
            Acks = GetAcks(options),
            ApiVersionRequest = options.ApiVersionRequest,
            BootstrapServers = input.Host,
            CompressionType = GetCompressionType(input),
            EnableIdempotence = options.EnableIdempotence,
            LingerMs = options.LingerMs,
            MaxInFlight = options.MaxInFlight,
            MessageTimeoutMs = options.MessageTimeoutMs,
            MessageMaxBytes = options.MessageMaxBytes,
            MessageSendMaxRetries = options.MessageSendMaxRetries,
            Partitioner = GetPartitioner(options),
            QueueBufferingMaxKbytes = options.QueueBufferingMaxKbytes,
            QueueBufferingMaxMessages = options.QueueBufferingMaxMessages,
            SecurityProtocol = GetSecurityProtocol(input),
            SocketTimeoutMs = socket.SocketTimeoutMs,
            SocketConnectionSetupTimeoutMs = socket.SocketConnectionSetupTimeoutMs,
            SocketKeepaliveEnable = socket.SocketKeepaliveEnable,
            SocketMaxFails = socket.SocketMaxFails,
            SocketNagleDisable = socket.SocketNagleDisable,
            SocketReceiveBufferBytes = socket.SocketReceiveBufferBytes,
            TransactionalId = options.TransactionalId,
            TransactionTimeoutMs = options.TransactionTimeoutMs,
        };

        if (sasl.UseSasl)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                throw new NotImplementedException("Sasl is implemented only only on Linux OS.");
            config.SaslMechanism = GetSaslMechanism(sasl);
            config.SaslUsername = sasl.SaslUsername;
            config.SaslPassword = sasl.SaslPassword;
            config.SaslOauthbearerMethod = GetSaslOauthbearerMethod(sasl);
            config.SaslOauthbearerClientId = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerClientId) ? "" : sasl.SaslOauthbearerClientId;
            config.SaslOauthbearerClientSecret = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerClientSecret) ? "" : sasl.SaslOauthbearerClientSecret;
            config.SaslOauthbearerTokenEndpointUrl = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerTokenEndpointUrl) ? "" : sasl.SaslOauthbearerTokenEndpointUrl;
            config.SaslOauthbearerConfig = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerConfig) ? "" : sasl.SaslOauthbearerConfig;
            config.SaslOauthbearerExtensions = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerExtensions) ? "" : sasl.SaslOauthbearerExtensions;
            config.SaslOauthbearerScope = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerScope) ? "" : sasl.SaslOauthbearerScope;
            config.SaslKerberosKeytab = string.IsNullOrWhiteSpace(sasl.SaslKerberosKeytab) ? "" : sasl.SaslKerberosKeytab;
            config.SaslKerberosMinTimeBeforeRelogin = sasl.SaslKerberosMinTimeBeforeRelogin;
            config.SaslKerberosPrincipal = sasl.SaslKerberosPrincipal;
            config.SaslKerberosServiceName = sasl.SaslKerberosServiceName;
        }

        if (ssl.UseSsl)
        {
            config.SslEndpointIdentificationAlgorithm = GetSslEndpointIdentificationAlgorithm(ssl);
            config.EnableSslCertificateVerification = ssl.EnableSslCertificateVerification;
            config.SslCertificateLocation = string.IsNullOrWhiteSpace(ssl.SslCertificateLocation) ? "" : ssl.SslCertificateLocation;
            config.SslCertificatePem = string.IsNullOrWhiteSpace(ssl.SslCertificatePem) ? "" : ssl.SslCertificatePem;
            if (!string.IsNullOrEmpty(ssl.SslCaCertificateStores))
                config.SslCaCertificateStores = ssl.SslCaCertificateStores;
            config.SslCaLocation = string.IsNullOrWhiteSpace(ssl.SslCaLocation) ? "" : ssl.SslCaLocation;
            config.SslCaPem = string.IsNullOrWhiteSpace(ssl.SslCaPem) ? "" : ssl.SslCaPem;
            config.SslKeyLocation = string.IsNullOrWhiteSpace(ssl.SslKeyLocation) ? "" : ssl.SslKeyLocation;
            config.SslKeyPassword = string.IsNullOrWhiteSpace(ssl.SslKeyPassword) ? "" : ssl.SslKeyPassword;
            config.SslKeyPem = string.IsNullOrWhiteSpace(ssl.SslKeyPem) ? "" : ssl.SslKeyPem;
            config.SslKeystoreLocation = string.IsNullOrWhiteSpace(ssl.SslKeystoreLocation) ? "" : ssl.SslKeystoreLocation;
            config.SslKeystorePassword = string.IsNullOrWhiteSpace(ssl.SslKeystorePassword) ? "" : ssl.SslKeystorePassword;
            config.SslEngineLocation = string.IsNullOrWhiteSpace(ssl.SslEngineLocation) ? "" : ssl.SslEngineLocation;
            config.SslCipherSuites = string.IsNullOrWhiteSpace(ssl.SslCipherSuites) ? "" : ssl.SslCipherSuites;
            config.SslCrlLocation = string.IsNullOrWhiteSpace(ssl.SslCrlLocation) ? "" : ssl.SslCrlLocation;
            config.SslCurvesList = string.IsNullOrWhiteSpace(ssl.SslCurvesList) ? "" : ssl.SslCurvesList;
            config.SslSigalgsList = string.IsNullOrWhiteSpace(ssl.SslSigalgsList) ? "" : ssl.SslSigalgsList;
        }

        return config;
    }
    private static SecurityProtocol GetSecurityProtocol(Input input)
    {
        return input.SecurityProtocol switch
        {
            SecurityProtocols.Plaintext => SecurityProtocol.Plaintext,
            SecurityProtocols.Ssl => SecurityProtocol.Ssl,
            SecurityProtocols.SaslPlaintext => SecurityProtocol.SaslPlaintext,
            SecurityProtocols.SaslSsl => SecurityProtocol.SaslSsl,
            _ => throw new Exception("SetSecurityProtocol error: Value not supported."),
        };
    }

    private static SslEndpointIdentificationAlgorithm GetSslEndpointIdentificationAlgorithm(Ssl ssl)
    {
        return ssl.SslEndpointIdentificationAlgorithm switch
        {
            SslEndpointIdentificationAlgorithms.None => SslEndpointIdentificationAlgorithm.None,
            SslEndpointIdentificationAlgorithms.Https => SslEndpointIdentificationAlgorithm.Https,
            _ => throw new Exception("SslEndpointIdentificationAlgorithm error: Value not supported."),
        };
    }

    private static CompressionType GetCompressionType(Input input)
    {
        return input.CompressionType switch
        {
            CompressionTypes.None => CompressionType.None,
            CompressionTypes.Gzip => CompressionType.Gzip,
            CompressionTypes.Snappy => CompressionType.Snappy,
            CompressionTypes.Lz4 => CompressionType.Lz4,
            CompressionTypes.Zstd => CompressionType.Zstd,
            _ => throw new Exception("SetCompressionType error: Value not supported."),
        };
    }

    private static Acks GetAcks(Options options)
    {
        return options.Acks switch
        {
            Ack.None => Acks.None,
            Ack.Leader => Acks.Leader,
            Ack.All => Acks.All,
            _ => throw new Exception("SetAcks error: Value not supported."),
        };
    }

    private static Partitioner GetPartitioner(Options options)
    {
        return options.Partitioner switch
        {
            Partitioners.Random => Partitioner.Random,
            Partitioners.Consistent => Partitioner.Consistent,
            Partitioners.ConsistentRandom => Partitioner.ConsistentRandom,
            Partitioners.Murmur2 => Partitioner.Murmur2,
            Partitioners.Murmur2Random => Partitioner.Murmur2Random,
            _ => throw new Exception("SetAcks error: Value not supported."),
        };
    }

    private static SaslOauthbearerMethod GetSaslOauthbearerMethod(Sasl sasl)
    {
        return sasl.SaslOauthbearerMethod switch
        {
            SaslOauthbearerMethods.Default => SaslOauthbearerMethod.Default,
            SaslOauthbearerMethods.Oidc => SaslOauthbearerMethod.Oidc,
            _ => throw new Exception("GetSaslOauthbearerMethod error: GetSaslOauthbearerMethod not supported."),
        };
    }

    private static SaslMechanism GetSaslMechanism(Sasl sasl)
    {
        return sasl.SaslMechanism switch
        {
            SaslMechanisms.Gssapi => SaslMechanism.Gssapi,
            SaslMechanisms.Plain => SaslMechanism.Plain,
            SaslMechanisms.ScramSha256 => SaslMechanism.ScramSha256,
            SaslMechanisms.ScramSha512 => SaslMechanism.ScramSha512,
            SaslMechanisms.OAuthBearer => SaslMechanism.OAuthBearer,
            _ => throw new Exception("GetSaslMechanism error: SaslMechanisms not supported."),
        };
    }
}