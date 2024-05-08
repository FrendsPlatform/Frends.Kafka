using Avro.Generic;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Frends.Kafka.Produce.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Runtime.InteropServices;

namespace Frends.Kafka.Produce.Tests;

[TestFixture]
public class UnitTests
{
    /*
        docker-compose up -d
         
        Read message(s) from topic:
        docker exec --interactive --tty broker kafka-console-consumer --bootstrap-server localhost:9092 --topic TestTopic --from-beginning
    */

    private readonly string? _hostPlaintext = "localhost:9092";
    private readonly string? _message = $"Hello {DateTime.Now}";
    private readonly string? _topic = "TestTopic";

    private Input _input = new();
    private Options _options = new();
    private Sasl _sasl = new() { UseSasl = false };
    private Ssl _ssl = new() { UseSsl = false };
    private SchemaRegistry _schemaRegistry = new() { UseSchemaRegistry = false };
    private readonly Socket _socket = new();

    [SetUp]
    public void Init()
    {
        _input = new Input()
        {
            Message = _message,
            Host = _hostPlaintext,
            Topic = _topic,
            CompressionType = CompressionTypes.None,
            Key = null,
            Partition = -1,
            SecurityProtocol = SecurityProtocols.Plaintext
        };

        _options = new Options()
        {
            MessageTimeoutMs = 20000,
            Acks = Ack.None,
            ApiVersionRequest = true,
            EnableIdempotence = false,
            LingerMs = 1000,
            MaxInFlight = 1000000,
            MessageMaxBytes = 1000000,
            MessageSendMaxRetries = 2147483647,
            Partitioner = Partitioners.ConsistentRandom,
            QueueBufferingMaxKbytes = 1048576,
            QueueBufferingMaxMessages = 100000,
            TransactionalId = null,
            TransactionTimeoutMs = 60000
        };
    }

    [Test]
    public async Task Kafka_Produce_Default_Test()
    {
        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, _schemaRegistry, default);
        ClassicAssert.That(result.Success);
        ClassicAssert.IsNotNull(result.Timestamp);
    }

    [Test]
    public async Task Kafka_Produce_Empty_Message()
    {
        _input.Message = null;
        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, _schemaRegistry, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsNotNull(result.Timestamp);
    }

    [Test]
    public async Task Kafka_Produce_Partition_Test()
    {
        _input.Partition = 0;
        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, _schemaRegistry, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsNotNull(result.Timestamp);
    }

    [Test]
    public async Task Kafka_Produce_Partition_Key()
    {
        _input.Key = "SomeKey";
        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, _schemaRegistry, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsNotNull(result.Timestamp);
    }

    [Test]
    public void Kafka_ErrorHandling_Test()
    {
        _options.MessageTimeoutMs = 1;
        var ex = ClassicAssert.ThrowsAsync<Exception>(() => Kafka.Produce(_input, _options, _socket, _sasl, _ssl, _schemaRegistry, default));
        ClassicAssert.IsNotNull(ex.Message);
    }

    [Test]
    public async Task Kafka_Produce_SSL()
    {
        _ssl = new Ssl
        {
            UseSsl = true,
            EnableSslCertificateVerification = true,
            SslCaCertificateStores = "",
            SslCaLocation = "",
            SslCaPem = "",
            SslCertificateLocation = "",
            SslCertificatePem = "",
            SslCipherSuites = "",
            SslCrlLocation = "",
            SslCurvesList = ""
        };

        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, _schemaRegistry, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsNotNull(result.Timestamp);
    }

    [Test]
    public async Task Kafka_ProduceSaSL()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            ClassicAssert.Ignore("Sasl is not supported on Windows.");
        else
        {
            _sasl = new Sasl
            {
                UseSasl = true,
                SaslMechanism = SaslMechanisms.Plain,
                SaslKerberosPrincipal = "",
                SaslKerberosServiceName = ""
            };

            var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, _schemaRegistry, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsNotNull(result.Timestamp);
        }
    }

    [Test]
    public async Task Kafka_Produce_Avro()
    {
        var schemaRegistry = new SchemaRegistry()
        {
            BasicAuthCredentialsSource = AuthCredentialsSources.UserInfo,
            BasicAuthUserInfo = null,
            EnableSslCertificateVerification = false,
            Records = @"{
  ""intField"": 123,
  ""longField"": 1234567890,
  ""floatField"": 1.23,
  ""doubleField"": 1.23456789,
  ""booleanField"": true,
  ""stringField"": ""test"",
  ""nullField"": null,
  ""bytesField"": ""dGVzdDE="", // ""test"" encoded in Base64
  ""enumField"": ""RED"",
  ""arrayField"": [""a"", ""b"", ""c""],
  ""mapField"": {""key1"": 1, ""key2"": 2, ""key3"": 3},
  ""fixedField"": ""YWJjZA=="", // ""abcd"" encoded in Base64
  ""unionField"": ""test"",
  ""recordField"": {
    ""nestedField"": ""test""
  }
}
",
            RecordSchemaJson = @"{
  ""type"": ""record"",
  ""name"": ""TestRecord"",
  ""namespace"": ""com.example"",
  ""fields"": [
    {
      ""name"": ""intField"",
      ""type"": ""int""
    },
    {
      ""name"": ""longField"",
      ""type"": ""long""
    },
    {
      ""name"": ""floatField"",
      ""type"": ""float""
    },
    {
      ""name"": ""doubleField"",
      ""type"": ""double""
    },
    {
      ""name"": ""booleanField"",
      ""type"": ""boolean""
    },
    {
      ""name"": ""stringField"",
      ""type"": ""string""
    },
    {
      ""name"": ""nullField"",
      ""type"": ""null""
    },
    {
      ""name"": ""bytesField"",
      ""type"": ""bytes""
    },
    {
      ""name"": ""enumField"",
      ""type"": {
        ""type"": ""enum"",
        ""name"": ""Colors"",
        ""symbols"": [""RED"", ""GREEN"", ""BLUE""]
      }
    },
    {
      ""name"": ""arrayField"",
      ""type"": {
        ""type"": ""array"",
        ""items"": ""string""
      }
    },
    {
      ""name"": ""mapField"",
      ""type"": {
        ""type"": ""map"",
        ""values"": ""int""
      }
    },
    {
      ""name"": ""fixedField"",
      ""type"": {
        ""type"": ""fixed"",
        ""name"": ""FourBytes"",
        ""size"": 4
      }
    },
    {
      ""name"": ""unionField"",
      ""type"": [""null"", ""string""]
    },
    {
      ""name"": ""recordField"",
      ""type"": {
        ""type"": ""record"",
        ""name"": ""NestedRecord"",
        ""fields"": [
          {
            ""name"": ""nestedField"",
            ""type"": ""string""
          }
        ]
      }
    }
  ]
}",

            UseSchemaRegistry = true,
            MaxCachedSchemas = 1000,
            RecordSchemaJsonFile = null,
            RequestTimeoutMs = 30000,
            SchemaRegistryUrl = "http://localhost:8081",
            SslCaLocation = null,
            SslKeystoreLocation = null,
            SslKeystorePassword = null
        };

        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, schemaRegistry, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsNotNull(result.Timestamp);

        // Consume the message
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _input.Host,
            GroupId = Guid.NewGuid().ToString(),
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var schemaRegistryConfig = new SchemaRegistryConfig()
        {
            Url = schemaRegistry.SchemaRegistryUrl,
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
            EnableSslCertificateVerification = schemaRegistry.EnableSslCertificateVerification,
            BasicAuthUserInfo = schemaRegistry.BasicAuthUserInfo,
            SslCaLocation = schemaRegistry.SslCaLocation,
            RequestTimeoutMs = schemaRegistry.RequestTimeoutMs,
            MaxCachedSchemas = schemaRegistry.MaxCachedSchemas,
            SslKeystorePassword = schemaRegistry.SslKeystorePassword,
            SslKeystoreLocation = schemaRegistry.SslKeystoreLocation,
        };

        using var cachedSchemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);

        using var consumer = new ConsumerBuilder<string, GenericRecord>(consumerConfig)
            .SetValueDeserializer(new AvroDeserializer<GenericRecord>(cachedSchemaRegistryClient).AsSyncOverAsync())
            .Build();

        consumer.Subscribe(_input.Topic);

        var consumedResult = consumer.Consume();

        // Verify the consumed message
        ClassicAssert.AreEqual(_input.Message, consumedResult.Message.Value);
    }
}