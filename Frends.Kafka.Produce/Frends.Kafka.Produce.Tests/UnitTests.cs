using Avro.Generic;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Frends.Kafka.Produce.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Text;

namespace Frends.Kafka.Produce.Tests;

[TestFixture]
public class UnitTests
{
    /*
        Update 05/2024:
            Using cloud Confluent Kafka. Set your own configs or see 'Basic testing with docker'. 
    
        Basic testing with docker:
            docker-compose up -d
            Read message(s) from topic:
            docker exec --interactive --tty broker kafka-console-consumer --bootstrap-server localhost:9092 --topic TestTopic --from-beginning
            Use Host = localhost:9092 and UseSasl = false
    */

    private readonly string _message = $"Hello {DateTime.Now}";
    private readonly string _topic = "ProduceBasicTestTopic";
    private readonly string? _bootstrapServers = Environment.GetEnvironmentVariable("ConfluentKafka_BootstrapServers");
    private readonly string? _apiKey = Environment.GetEnvironmentVariable("ConfluentKafka_APIKey");
    private readonly string? _apiKeySecret = Environment.GetEnvironmentVariable("ConfluentKafka_APIKeySecret");
    private readonly string? _schemaRegistryURL = Environment.GetEnvironmentVariable("ConfluentKafka_SchemaRegistryURL");
    private readonly string? _schemaRegistryAPIKey = Environment.GetEnvironmentVariable("ConfluentKafka_SchemaRegistryAPIKey");
    private readonly string? _schemaRegistryAPIKeySecret = Environment.GetEnvironmentVariable("ConfluentKafka_SchemaRegistryAPIKeySecret");

    private Input _input = new();
    private Options _options = new();
    private Sasl _sasl = new();
    private Socket _socket = new();
    private Ssl _ssl = new();
    private SchemaRegistry _schemaRegistry = new();

    [SetUp]
    public void Init()
    {
        _input = new Input()
        {
            Message = _message,
            Host = _bootstrapServers,
            Topic = _topic,
            CompressionType = CompressionTypes.None,
            Key = null,
            Partition = -1,
            SecurityProtocol = SecurityProtocols.SaslSsl
        };

        _options = new Options()
        {
            MessageTimeoutMs = 20000,
            Acks = Ack.None,
            EnableIdempotence = false,
            LingerMs = 1000,
            MaxInFlight = 1000000,
            MessageMaxBytes = 1000000,
            MessageSendMaxRetries = 2147483647,
            Partitioner = Partitioners.ConsistentRandom,
            QueueBufferingMaxKbytes = 1048576,
            QueueBufferingMaxMessages = 100000,
            TransactionalId = null,
            TransactionTimeoutMs = 60000,
            Debug = "all"
        };

        _schemaRegistry = new SchemaRegistry()
        {
            UseSchemaRegistry = false,
            BasicAuthCredentialsSource = AuthCredentialsSources.UserInfo,
            BasicAuthUserInfo = $"{_schemaRegistryAPIKey}:{_schemaRegistryAPIKeySecret}",
            EnableSslCertificateVerification = false,
            MaxCachedSchemas = 1000,
            Records = @"{
  ""intField"": 123,
  ""longField"": 1234567890,
  ""floatField"": 1.23,
  ""doubleField"": 1.23456789,
  ""booleanField"": true,
  ""stringField"": ""Hello, World!"",
  ""nullField"": null,
  ""bytesField"": ""dGVzdDE="",
  ""enumField"": ""RED"",
  ""arrayField"": [""item1"", ""item2"", ""item3""],
  ""mapField"": {
    ""key1"": 1,
    ""key2"": 2,
    ""key3"": 3
  },
  ""fixedField"": ""YWJjZA=="",
  ""unionField"": ""Hello, Union!"",
  ""recordField"": {
    ""nestedField"": ""Hello, Nested!""
  }
}
",
            SchemaJson = @"{
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
        ""name"": ""Colors"",
        ""symbols"": [
          ""RED"",
          ""GREEN"",
          ""BLUE""
        ],
        ""type"": ""enum""
      }
    },
    {
      ""name"": ""arrayField"",
      ""type"": {
        ""items"": ""string"",
        ""type"": ""array""
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
        ""name"": ""FourBytes"",
        ""size"": 4,
        ""type"": ""fixed""
      }
    },
    {
      ""name"": ""unionField"",
      ""type"": [
        ""null"",
        ""string""
      ]
    },
    {
      ""name"": ""recordField"",
      ""type"": {
        ""fields"": [
          {
            ""name"": ""nestedField"",
            ""type"": ""string""
          }
        ],
        ""name"": ""NestedRecord"",
        ""type"": ""record""
      }
    }
  ],
  ""name"": ""sampleRecord"",
  ""namespace"": ""com.mycorp.mynamespace"",
  ""type"": ""record""
}",
            RequestTimeoutMs = 30000,
            SchemaRegistryUrl = _schemaRegistryURL,
            SslCaLocation = null,
            SslKeystoreLocation = null,
            SslKeystorePassword = null,
        };

        _socket = new Socket()
        {
            SocketTimeoutMs = 60000,
            SocketConnectionSetupTimeoutMs = 30000,
            SocketKeepaliveEnable = false,
            SocketMaxFails = 1,
            SocketNagleDisable = false,
            SocketReceiveBufferBytes = 0
        };

        // SASL is required for cloud tests.
        _sasl = new Sasl()
        {
            UseSasl = true,
            SaslMechanism = SaslMechanisms.Plain,
            SaslUsername = _apiKey,
            SaslPassword = _apiKeySecret,
        };

        _ssl = new Ssl()
        {
            UseSsl = false,
            SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithms.None,
            EnableSslCertificateVerification = true,
            SslCaCertificateStores = "root",
            SslCaLocation = "root",
            SslCaPem = null,
            SslCertificateLocation = null,
            SslCertificatePem = null,
            SslCipherSuites = null,
            SslCrlLocation = null,
            SslCurvesList = null,
            SslEngineLocation = null,
            SslKeyLocation = null,
            SslKeyPassword = null,
            SslKeyPem = null,
            SslKeystoreLocation = null,
            SslKeystorePassword = null,
            SslSigalgsList = null,
        };
    }

    [TearDown]
    public void Cleanup()
    {
        //For manual cleaning up. Run until ConsumeBasic() returns nothing for ~10 seconds.
        //while (true)
        //    ConsumeBasic();
    }

    [Test]
    public async Task Kafka_Produce_Default_Test()
    {
        var result = await Kafka.Produce(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.That(result.Success);
        ClassicAssert.IsNotNull(result.Data);
        ClassicAssert.AreEqual(_input.Message, ConsumeBasic().Message.Value);
    }

    [Test]
    public async Task Kafka_Produce_CompressionTypes()
    {
        var values = new[] { CompressionTypes.Gzip, CompressionTypes.Lz4, CompressionTypes.None, CompressionTypes.Snappy, CompressionTypes.Zstd };

        foreach (var item in values)
        {
            _input.CompressionType = item;
            var result = await Kafka.Produce(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.That(result.Success);
            ClassicAssert.IsNotNull(result.Data);
            ClassicAssert.AreEqual(_input.Message, ConsumeBasic().Message.Value);
        }
    }

    [Test]
    public async Task Kafka_Produce_Empty_Message()
    {
        _input.Message = null;
        var result = await Kafka.Produce(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsNotNull(result.Data);
        ClassicAssert.AreEqual(_input.Message, ConsumeBasic().Message.Value);
    }

    [Test]
    public async Task Kafka_Produce_Partition_Test()
    {
        var values = new[] { -1, 2 };

        foreach (var item in values)
        {
            _input.Partition = item;
            var result = await Kafka.Produce(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.That(result.Success);
            ClassicAssert.IsNotNull(result.Data);
            ClassicAssert.AreEqual(_input.Message, ConsumeBasic().Message.Value);
        }
    }

    [Test]
    public async Task Kafka_Produce_Partition_Key()
    {
        _input.Key = "SomeKey";
        var result = await Kafka.Produce(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsNotNull(result.Data);
        ClassicAssert.AreEqual(_input.Message, ConsumeBasic().Message.Value);
    }

    [Test]
    [Ignore("Can't test with cloud.")]
    public async Task Kafka_Produce_SecurityProtocols_Plaintext_Test()
    {
        _input.SecurityProtocol = SecurityProtocols.Plaintext;
        var result = await Kafka.Produce(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.That(result.Success);
        ClassicAssert.IsNotNull(result.Data);
        ClassicAssert.AreEqual(_input.Message, ConsumeBasic().Message.Value);
    }

    [Test]
    [Ignore("Can't test with cloud.")]
    public async Task Kafka_Produce_SecurityProtocols_SSL_Test()
    {
        _input.SecurityProtocol = SecurityProtocols.Ssl;
        var result = await Kafka.Produce(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.That(result.Success);
        ClassicAssert.IsNotNull(result.Data);
        ClassicAssert.AreEqual(_input.Message, ConsumeBasic().Message.Value);
    }

    [Test]
    public void Kafka_ErrorHandling_Test()
    {
        _options.MessageTimeoutMs = 1;
        var ex = ClassicAssert.ThrowsAsync<InvalidOperationException>(() => Kafka.Produce(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
    }

    [Test]
    public async Task Kafka_Produce_Avro_Valid()
    {
        _input.Topic = "ProduceAvroTestTopic";
        _schemaRegistry.UseSchemaRegistry = true;

        var result = await Kafka.Produce(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsNotNull(result.Data);

        var consumedRecord = ConsumeAvro().Message.Value;
        ClassicAssert.AreEqual(123, consumedRecord["intField"]);
        ClassicAssert.AreEqual(1234567890, consumedRecord["longField"]);
        ClassicAssert.AreEqual(1.23f, consumedRecord["floatField"]);
        ClassicAssert.AreEqual(1.23456789, consumedRecord["doubleField"]);
        ClassicAssert.AreEqual(true, consumedRecord["booleanField"]);
        ClassicAssert.AreEqual("Hello, World!", consumedRecord["stringField"]);
        ClassicAssert.AreEqual(null, consumedRecord["nullField"]);
        ClassicAssert.AreEqual("RED", ((GenericEnum)consumedRecord["enumField"]).Value);
        CollectionAssert.AreEqual(new List<string> { "item1", "item2", "item3" }, ((object[])consumedRecord["arrayField"]).Cast<string>().ToList());
        ClassicAssert.AreEqual("Hello, Union!", consumedRecord["unionField"]);
        ClassicAssert.AreEqual("Hello, Nested!", ((GenericRecord)consumedRecord["recordField"])["nestedField"]);

        var actualBytes = (byte[])consumedRecord["bytesField"];
        var actualBase64 = Encoding.ASCII.GetString(actualBytes);
        var actualString = Encoding.ASCII.GetString(Convert.FromBase64String(actualBase64));
        ClassicAssert.AreEqual("test1", actualString);

        var expectedBytes = Encoding.ASCII.GetBytes("abcd");
        var actualBytes2 = ((GenericFixed)consumedRecord["fixedField"]).Value;
        CollectionAssert.AreEqual(expectedBytes, actualBytes2);

        var expectedMap = new Dictionary<string, int> { { "key1", 1 }, { "key2", 2 }, { "key3", 3 } };
        var actualMap = ((Dictionary<string, object>)consumedRecord["mapField"]).ToDictionary(kvp => kvp.Key, kvp => Convert.ToInt32(kvp.Value));
        CollectionAssert.AreEqual(expectedMap, actualMap);
    }

    [Test]
    public void Kafka_Produce_Avro_InValid_intFieldShouldNotBeString()
    {
        _input.Topic = "TaskTestSchemaTopic";

        _schemaRegistry.UseSchemaRegistry = true;
        _schemaRegistry.Records = @"{
  ""intField"": ""asd"",
  ""longField"": 1234567890,
  ""floatField"": 1.23,
  ""doubleField"": 1.23456789,
  ""booleanField"": true,
  ""stringField"": ""Hello, World!"",
  ""nullField"": null,
  ""bytesField"": ""dGVzdDE="",
  ""enumField"": ""RED"",
  ""arrayField"": [""item1"", ""item2"", ""item3""],
  ""mapField"": {
    ""key1"": 1,
    ""key2"": 2,
    ""key3"": 3
  },
  ""fixedField"": ""YWJjZA=="",
  ""unionField"": ""Hello, Union!"",
  ""recordField"": {
    ""nestedField"": ""Hello, Nested!""
  }
}
";
        _schemaRegistry.SchemaJson = @"{
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
        ""name"": ""Colors"",
        ""symbols"": [
          ""RED"",
          ""GREEN"",
          ""BLUE""
        ],
        ""type"": ""enum""
      }
    },
    {
      ""name"": ""arrayField"",
      ""type"": {
        ""items"": ""string"",
        ""type"": ""array""
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
        ""name"": ""FourBytes"",
        ""size"": 4,
        ""type"": ""fixed""
      }
    },
    {
      ""name"": ""unionField"",
      ""type"": [
        ""null"",
        ""string""
      ]
    },
    {
      ""name"": ""recordField"",
      ""type"": {
        ""fields"": [
          {
            ""name"": ""nestedField"",
            ""type"": ""string""
          }
        ],
        ""name"": ""NestedRecord"",
        ""type"": ""record""
      }
    }
  ],
  ""name"": ""sampleRecord"",
  ""namespace"": ""com.mycorp.mynamespace"",
  ""type"": ""record""
}";
        ClassicAssert.ThrowsAsync<FormatException>(() => Kafka.Produce(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
    }

    private ConsumeResult<string, string> ConsumeBasic()
    {
        using var consumer = new ConsumerBuilder<string, string>(GetConsumeConfig()).Build();
        consumer.Subscribe(_input.Topic);
        var cr = consumer.Consume();
        consumer.Close();
        return cr;
    }

    private ConsumeResult<string, GenericRecord> ConsumeAvro()
    {
        var schemaRegistryConfig = new SchemaRegistryConfig()
        {
            Url = _schemaRegistry.SchemaRegistryUrl,
            BasicAuthCredentialsSource = (AuthCredentialsSource?)_schemaRegistry.BasicAuthCredentialsSource,
            EnableSslCertificateVerification = _schemaRegistry.EnableSslCertificateVerification,
            BasicAuthUserInfo = _schemaRegistry.BasicAuthUserInfo,
        };

        using var cachedSchemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
        using var consumer = new ConsumerBuilder<string, GenericRecord>(GetConsumeConfig())
            .SetValueDeserializer(new AvroDeserializer<GenericRecord>(cachedSchemaRegistryClient).AsSyncOverAsync())
            .Build();
        {
            consumer.Subscribe(_input.Topic);
            // consumes messages from the subscribed topic and prints them to the console
            var cr = consumer.Consume();
            var consumedRecord = cr.Message.Value;
            consumer.Close();
            return cr;
        }
    }

    private ConsumerConfig GetConsumeConfig()
    {
        return new ConsumerConfig()
        {
            GroupId = "csharp-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            SaslUsername = _apiKey,
            SaslPassword = _apiKeySecret,
            BootstrapServers = _bootstrapServers,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain
        };
    }
}