using Confluent.Kafka;
using Frends.Kafka.Consume.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Frends.Kafka.Consume.Tests;

[TestFixture]
public class TestSchemaRegistry
{
    /*
        Update 05/2024:
            Using cloud Confluent Kafka. Set your own configs or see 'Basic testing with docker'. 
    
        Basic testing with docker:
        Docker compose:
        Run command 'docker-compose up -d' in .\Frends.Kafka.Consume.Tests
        
        Read message(s) from topic:
        docker exec --interactive --tty "container's name" kafka-console-consumer --bootstrap-server localhost:9092 --topic ConsumeTopic --from-beginning
    */

    private readonly string _message = $"Hello {DateTime.Now}";
    private readonly string _topic = "ConsumeAvroTest";
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

    private readonly string _records = @"{
  ""intField"": 1,
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
    private readonly string _schema = @"{
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

    [SetUp]
    public void Setup()
    {
        _input = new Input()
        {
            Host = _bootstrapServers,
            Topic = _topic,
            SecurityProtocol = SecurityProtocols.SaslSsl,
            MessageCount = 10,
            Timeout = 10000,
            Partition = -1,
        };

        _options = new Options()
        {
            Acks = Ack.None,
            MaxInFlight = 1000000,
            MessageMaxBytes = 1000000,
            ConnectionsMaxIdleMs = 10000,
            AutoCommitIntervalMs = 5000,
            AutoOffsetReset = AutoOffsetResets.Earliest,
            BrokerAddressFamily = BrokerAddressFamilys.Any,
            EnableAutoCommit = true,
            EnableAutoOffsetStore = true,
            CheckCrcs = false,
            FetchErrorBackoffMs = 500,
            FetchMaxBytes = 52428800,
            FetchMinBytes = 1,
            FetchWaitMaxMs = 500,
            GroupId = "csharp-group-1",
            GroupInstanceId = null,
            HeartbeatIntervalMs = 3000,
            IsolationLevel = IsolationLevels.ReadUncommitted,
            QueuedMaxMessagesKbytes = 65536,
            QueuedMinMessages = 1,
            ReconnectBackoffMaxMs = 10000,
            ReconnectBackoffMs = 100,
            SessionTimeoutMs = 10000,
            MaxPollIntervalMs = 20000,
            Debug = "all",
            EncodeMessageKey = true,
        };

        _socket = new Socket()
        {
            SocketConnectionSetupTimeoutMs = 30000,
            SocketKeepaliveEnable = false,
            SocketMaxFails = 1,
            SocketNagleDisable = false,
            SocketReceiveBufferBytes = 0,
            SocketTimeoutMs = 10000,
        };

        _schemaRegistry = new SchemaRegistry()
        {
            UseSchemaRegistry = true,
            BasicAuthCredentialsSource = AuthCredentialsSources.UserInfo,
            BasicAuthUserInfo = $"{_schemaRegistryAPIKey}:{_schemaRegistryAPIKeySecret}",
            MaxCachedSchemas = 1000,
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
            SocketReceiveBufferBytes = 0,
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

    //[TearDown]
    //public void Cleanup()
    //{
    //    //_schemaRegistry.UseSchemaRegistry = true;
    //    //while(true)
    //    //    Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
    //}

    [Test]
    public async Task Kafka_Consume_Avro_Test()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public void Kafka_Consume_Avro_InvalidTopic()
    {
        _input.Topic = "invalidTopic";
        var ex = ClassicAssert.Throws<Confluent.Kafka.ConsumeException>(() => Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
        ClassicAssert.NotNull(ex);
        ClassicAssert.AreEqual("Subscribed topic not available: invalidTopic: Broker: Unknown topic or partition", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_MessageCount_Random()
    {
        var rndCount = new Random().Next(0, 6);
        _input.MessageCount = rndCount;

        for (int i = 0; i < rndCount; i++)
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(rndCount, result.Data.Count);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    [Ignore("Manual test.")]
    public async Task Kafka_Consume_Avro_MessageCount_DontReadAll()
    {
        var rndCount = new Random().Next(0, 6);
        _input.MessageCount = rndCount;
        var extras = new Random().Next(0, 3);

        for (int i = 0; i < rndCount + extras; i++)
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(rndCount, result.Data.Count);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));

        var result2 = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result2.Success);
        ClassicAssert.AreEqual(extras, result2.Data.Count);
        ClassicAssert.IsTrue(result2.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_MessageCount_Unlimited()
    {
        var rndCount = new Random().Next(0, 6);
        _input.MessageCount = 0;

        for (int i = 0; i < rndCount; i++)
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(rndCount, result.Data.Count);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_Timeout_Unlimited()
    {
        _input.Timeout = 0;
        var rndCount = new Random().Next(0, 6);
        _input.MessageCount = rndCount;

        for (int i = 0; i < rndCount; i++)
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(rndCount, result.Data.Count);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_SecurityProtocol_SaslSsl()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _input.SecurityProtocol = SecurityProtocols.SaslSsl;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    /// This test won't fail but doesn't return anything either because cloud likes SaslSsl.
    [Test]
    public async Task Kafka_Consume_Avro_SecurityProtocol_Plaintext()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _input.SecurityProtocol = SecurityProtocols.Plaintext;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    /// This test won't fail but doesn't return anything either because cloud likes SaslSsl.
    [Test]
    public async Task Kafka_Consume_Avro_SecurityProtocol_SSL()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _input.SecurityProtocol = SecurityProtocols.Ssl;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public async Task Kafka_Consume_Avro_Partition()
    {
        var parameterLooper = new[] { -1, 2 };

        foreach (var item in parameterLooper)
        {
            _input.Partition = item;
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_Ack()
    {
        var parameterLooper = new[] { Ack.All, Ack.None, Ack.Leader };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.Acks = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_AutoCommitIntervalMs()
    {
        var parameterLooper = new[] { 0, 1000 };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.AutoCommitIntervalMs = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_AutoOffsetReset()
    {
        var parameterLooper = new[] { AutoOffsetResets.Latest, AutoOffsetResets.Earliest };

        foreach (var item in parameterLooper)
        {
            _options.AutoOffsetReset = item;
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_BrokerAddressFamilys()
    {
        var parameterLooper = new[] { BrokerAddressFamilys.V6, BrokerAddressFamilys.V4, BrokerAddressFamilys.Any };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.BrokerAddressFamily = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");

            if (item != BrokerAddressFamilys.V6)
                ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_CheckCrcs()
    {
        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.CheckCrcs = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Avro_ConnectionsMaxIdleMs()
    {
        var parameterLooper = new[] { 0, 1000 };

        foreach (var item in parameterLooper)
        {
            _options.ConnectionsMaxIdleMs = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_Debug()
    {
        var parameterLooper = new[] { string.Empty, null, "consumer,cgrp,topic,fetch", "all" };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.Debug = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_EnableAutoCommit()
    {
        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.EnableAutoCommit = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_EnableAutoOffsetStore()
    {
        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.EnableAutoOffsetStore = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_EncodeMessageKey()
    {
        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.EncodeMessageKey = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchErrorBackoffMs()
    {
        var parameterLooper = new[] { 0, 1000 };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.FetchErrorBackoffMs = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchMaxBytes()
    {

        _options.FetchMaxBytes = 52428800;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchMaxBytes_0()
    {

        _options.FetchMaxBytes = 0;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var ex = ClassicAssert.Throws<InvalidOperationException>(() => Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
        ClassicAssert.AreEqual("`fetch.max.bytes` must be >= `message.max.bytes`", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchMaxBytes_MessageMaxBytes_0()
    {

        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _options.FetchMaxBytes = 0;
        _options.MessageMaxBytes = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
        ClassicAssert.AreEqual("Configuration property \"message.max.bytes\" value 0 is outside allowed range 1000..1000000000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_MaxInFlight()
    {

        _options.MaxInFlight = 1000000;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_MaxInFlight_0()
    {

        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _options.MaxInFlight = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
        ClassicAssert.AreEqual("Configuration property \"max.in.flight.requests.per.connection\" value 0 is outside allowed range 1..1000000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_QueuedMaxMessagesKbytes()
    {

        _options.QueuedMaxMessagesKbytes = 65536;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_QueuedMaxMessagesKbytes_0()
    {

        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _options.QueuedMaxMessagesKbytes = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
        ClassicAssert.AreEqual("Configuration property \"queued.max.messages.kbytes\" value 0 is outside allowed range 1..2097151\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_QueuedMinMessages()
    {

        _options.QueuedMinMessages = 65536;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_QueuedMinMessages_0()
    {

        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _options.QueuedMinMessages = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
        ClassicAssert.AreEqual("Configuration property \"queued.min.messages\" value 0 is outside allowed range 1..10000000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchMinBytes()
    {

        _options.FetchMinBytes = 1;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchMinBytes_0()
    {

        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _options.FetchMinBytes = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
        ClassicAssert.AreEqual("Configuration property \"fetch.min.bytes\" value 0 is outside allowed range 1..100000000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchWaitMaxMs()
    {
        var parameterLooper = new[] { 0, 500 };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.FetchWaitMaxMs = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Avro_FetchWaitMaxMs_NoMessage()
    {

        _options.FetchWaitMaxMs = 500;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(0, result.Data.Count);
    }

    [Test]
    public async Task Kafka_Consume_Avro_GroupId()
    {
        var parameterLooper = new[] { "csharp-group-1", "foo" };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.GroupId = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_GroupId_Foo()
    {

        _options.GroupId = "foo";
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public async Task Kafka_Consume_Avro_GroupId_StringEmpty()
    {

        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _options.GroupId = string.Empty;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
        ClassicAssert.AreEqual("'group.id' configuration parameter is required and was not specified.", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_GroupId_Null()
    {

        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _options.GroupId = null;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
        ClassicAssert.AreEqual("'group.id' configuration parameter is required and was not specified.", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_GroupInstanceId()
    {
        var parameterLooper = new[] { string.Empty, null, "foo", "csharp-group-1" };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.GroupInstanceId = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Avro_HeartbeatIntervalMs()
    {

        _options.HeartbeatIntervalMs = 3000;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public void Kafka_Consume_Avro_HeartbeatIntervalMs_0()
    {

        _options.HeartbeatIntervalMs = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
        ClassicAssert.AreEqual("Configuration property \"heartbeat.interval.ms\" value 0 is outside allowed range 1..3600000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_IsolationLevels()
    {
        var parameterLooper = new[] { IsolationLevels.ReadCommitted, IsolationLevels.ReadUncommitted };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.IsolationLevel = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Avro_ReconnectBackoffMaxMs()
    {

        _options.ReconnectBackoffMaxMs = 10000;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public void Kafka_Consume_Avro_ReconnectBackoffMaxMs_0()
    {

        _options.ReconnectBackoffMaxMs = 0;

        var ex = ClassicAssert.Throws<InvalidOperationException>(() => Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default));
        ClassicAssert.AreEqual("`reconnect.backoff.max.ms` must be >= `reconnect.max.ms`", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_ReconnectBackoffMs()
    {
        var parameterLooper = new[] { 0, 100 };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _options.ReconnectBackoffMs = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_SaslMechanisms_Plain()
    {

        _sasl.SaslMechanism = SaslMechanisms.Plain;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Avro_SaslMechanisms_Gssapi()
    {

        _sasl.SaslMechanism = SaslMechanisms.Gssapi;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Avro_SaslMechanisms_ScramSha512()
    {

        _sasl.SaslMechanism = SaslMechanisms.ScramSha512;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Avro_SaslMechanisms_ScramSha256()
    {

        _sasl.SaslMechanism = SaslMechanisms.ScramSha256;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Avro_SaslMechanisms_OAuthBearer()
    {

        _sasl.SaslMechanism = SaslMechanisms.OAuthBearer;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // There's no error if invalid creds.
    [Test]
    public void Kafka_Consume_Avro_SaslUsername_Invalid()
    {

        _sasl.SaslUsername = "foo";

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // There's no error if invalid creds.
    [Test]
    public void Kafka_Consume_Avro_SaslPassword_Invalid()
    {

        _sasl.SaslPassword = "foo";

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public async Task Kafka_Consume_Avro_SaslOauthbearerMethods()
    {
        var parameterLooper = new[] { SaslOauthbearerMethods.Default, SaslOauthbearerMethods.Oidc };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _sasl.SaslOauthbearerMethod = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_SaslOauthbearerClientId_Oidc()
    {

        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
        _sasl.SaslOauthbearerClientId = "foo";

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_SaslOauthbearerClientSecret_Oidc()
    {

        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
        _sasl.SaslOauthbearerClientSecret = "foo";

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_Oidc_Secret_ID_Null()
    {

        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
        _sasl.SaslOauthbearerClientSecret = null;
        _sasl.SaslOauthbearerClientId = null;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_Oidc_SaslOauthbearerTokenEndpointUrl()
    {
        var parameterLooper = new[] { null, "localhost", string.Empty };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            _sasl.SaslOauthbearerTokenEndpointUrl = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_Oidc_SaslOauthbearerExtensions()
    {
        var parameterLooper = new[] { null, "supportFeatureX=true,organizationId=sales-emea", string.Empty };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            _sasl.SaslOauthbearerExtensions = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_Oidc_SaslOauthbearerScope()
    {
        var parameterLooper = new[] { null, "ExampleScope", string.Empty };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            _sasl.SaslOauthbearerScope = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_Oidc_SaslOauthbearerConfig()
    {
        var parameterLooper = new[] { null, "principal=admin extension_traceId=123", string.Empty };

        foreach (var item in parameterLooper)
        {

            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            _sasl.SaslOauthbearerConfig = item;

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_Socket_SocketKeepaliveEnable()
    {

        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {
            _socket.SocketKeepaliveEnable = item;
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"looper: {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_Socket_SocketNagleDisable()
    {

        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {
            _socket.SocketNagleDisable = item;
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"looper: {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_Socket_SocketTimeoutMs()
    {
        _socket.SocketTimeoutMs = 10;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_Socket_SocketConnectionSetupTimeoutMs()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
        _socket.SocketConnectionSetupTimeoutMs = 1000;

        var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_Socket_SocketMaxFails()
    {
        var parameterLooper = new[] { 0, 1 };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);
            _socket.SocketMaxFails = item;
            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"looper: {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_Socket_SocketReceiveBufferBytes()
    {

        var parameterLooper = new[] { 0, 1, 2 };

        foreach (var item in parameterLooper)
        {
            _socket.SocketReceiveBufferBytes = item;
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: _records, schema: _schema);

            var result = Kafka.Consume(_input, _socket, _sasl, _ssl, _schemaRegistry, _options, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"looper: {item}");
        }
    }

    private async Task ProduceTestMessage(bool isBasic, string? msgKey, string? msg, string? records, string? schema)
    {
        if (isBasic)
            await MessageProducer.ProduceBasic(_input, _options, _socket, _sasl, _ssl, msgKey, msg, default);
        else
            await MessageProducer.ProduceAvro(_input, _options, _socket, _sasl, _ssl, records, schema, msgKey, _schemaRegistry, default);
    }
}