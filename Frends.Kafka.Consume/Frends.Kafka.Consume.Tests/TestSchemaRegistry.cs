using Confluent.Kafka;
using Frends.Kafka.Consume.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Frends.Kafka.Consume.Tests;

[TestFixture]
public class TestSchemaRegistry : TestBase
{
    protected override string Topic => "ConsumeAvroTest";

    protected override bool UseSchemaRegistry => true;

    private readonly string records = @"{
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

    private readonly string schema = @"{
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

    [Test]
    public async Task Kafka_Consume_Avro_Test()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public void Kafka_Consume_Avro_InvalidTopic()
    {
        input.Topic = "invalidTopic";
        var ex = ClassicAssert.Throws<Confluent.Kafka.ConsumeException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.NotNull(ex);
        ClassicAssert.AreEqual("Subscribed topic not available: invalidTopic: Broker: Unknown topic or partition", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_MessageCount_Random()
    {
        var rndCount = new Random().Next(0, 6);
        input.MessageCount = rndCount;

        for (int i = 0; i < rndCount; i++)
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(rndCount, result.Data.Count);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    [NUnit.Framework.Ignore("Manual test.")]
    public async Task Kafka_Consume_Avro_MessageCount_DontReadAll()
    {
        var rndCount = new Random().Next(0, 6);
        input.MessageCount = rndCount;
        var extras = new Random().Next(0, 3);

        for (int i = 0; i < rndCount + extras; i++)
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(rndCount, result.Data.Count);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));

        var result2 = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result2.Success);
        ClassicAssert.AreEqual(extras, result2.Data.Count);
        ClassicAssert.IsTrue(result2.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_MessageCount_Unlimited()
    {
        var rndCount = new Random().Next(1, 6);
        input.MessageCount = 0;

        for (int i = 0; i < rndCount; i++)
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);

        // We can't know the exact count, but it should be at least rndCount.
        ClassicAssert.IsTrue(result.Data.Count >= rndCount);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_Timeout_Unlimited()
    {
        input.Timeout = 0;
        var rndCount = new Random().Next(0, 6);
        input.MessageCount = rndCount;

        for (int i = 0; i < rndCount; i++)
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(rndCount, result.Data.Count);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_SecurityProtocol_SaslSsl()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        input.SecurityProtocol = SecurityProtocols.SaslSsl;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    // This test won't fail but doesn't return anything either because cloud likes SaslSsl.
    [Test]
    public async Task Kafka_Consume_Avro_SecurityProtocol_Plaintext()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        input.SecurityProtocol = SecurityProtocols.Plaintext;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // This test won't fail but doesn't return anything either because cloud likes SaslSsl.
    [Test]
    public async Task Kafka_Consume_Avro_SecurityProtocol_SSL()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        input.SecurityProtocol = SecurityProtocols.Ssl;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public async Task Kafka_Consume_Avro_Partition()
    {
        var parameterLooper = new[] { -1, 2 };

        foreach (var item in parameterLooper)
        {
            input.Partition = item;
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

            var result = Kafka.Consume(input, Conn(), options, default);
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
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.Acks = item;

            var result = Kafka.Consume(input, Conn(), options, default);
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
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.AutoCommitIntervalMs = item;

            var result = Kafka.Consume(input, Conn(), options, default);
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
            options.AutoOffsetReset = item;
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            if (item != AutoOffsetResets.Latest)
                ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_BrokerAddressFamilys()
    {
        var parameterLooper = new[] { BrokerAddressFamilys.V6, BrokerAddressFamilys.V4, BrokerAddressFamilys.Any };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.BrokerAddressFamily = item;

            var result = Kafka.Consume(input, Conn(), options, default);
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
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.CheckCrcs = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Avro_ConnectionsMaxIdleMs()
    {
        var parameterLooper = new[] { 0, 1000 };

        foreach (var item in parameterLooper)
        {
            options.ConnectionsMaxIdleMs = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_Debug()
    {
        var parameterLooper = new[] { string.Empty, null, "consumer,cgrp,topic,fetch", "all" };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.Debug = item;

            var result = Kafka.Consume(input, Conn(), options, default);
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
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.EnableAutoCommit = item;

            var result = Kafka.Consume(input, Conn(), options, default);
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
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.EnableAutoOffsetStore = item;

            var result = Kafka.Consume(input, Conn(), options, default);
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
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.EncodeMessageKey = item;

            var result = Kafka.Consume(input, Conn(), options, default);
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
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.FetchErrorBackoffMs = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchMaxBytes()
    {
        options.FetchMaxBytes = 52428800;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchMaxBytes_0()
    {
        options.FetchMaxBytes = 0;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var ex = ClassicAssert.Throws<InvalidOperationException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("`fetch.max.bytes` must be >= `message.max.bytes`", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchMaxBytes_MessageMaxBytes_0()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        options.FetchMaxBytes = 0;
        options.MessageMaxBytes = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"message.max.bytes\" value 0 is outside allowed range 1000..1000000000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_MaxInFlight()
    {
        options.MaxInFlight = 1000000;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_MaxInFlight_0()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        options.MaxInFlight = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"max.in.flight.requests.per.connection\" value 0 is outside allowed range 1..1000000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_QueuedMaxMessagesKbytes()
    {
        options.QueuedMaxMessagesKbytes = 65536;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_QueuedMaxMessagesKbytes_0()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        options.QueuedMaxMessagesKbytes = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"queued.max.messages.kbytes\" value 0 is outside allowed range 1..2097151\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_QueuedMinMessages()
    {
        options.QueuedMinMessages = 65536;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_QueuedMinMessages_0()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        options.QueuedMinMessages = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"queued.min.messages\" value 0 is outside allowed range 1..10000000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchMinBytes()
    {
        options.FetchMinBytes = 1;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchMinBytes_0()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        options.FetchMinBytes = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"fetch.min.bytes\" value 0 is outside allowed range 1..100000000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_FetchWaitMaxMs()
    {
        var parameterLooper = new[] { 0, 500 };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.FetchWaitMaxMs = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Avro_FetchWaitMaxMs_NoMessage()
    {
        options.FetchWaitMaxMs = 500;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(0, result.Data.Count);
    }

    [Test]
    public async Task Kafka_Consume_Avro_GroupId()
    {
        var parameterLooper = new[] { "csharp-group-1", "foo" };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.GroupId = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_GroupId_Foo()
    {
        options.GroupId = "foo";
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public async Task Kafka_Consume_Avro_GroupId_StringEmpty()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        options.GroupId = string.Empty;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("'group.id' configuration parameter is required and was not specified.", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_GroupId_Null()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        options.GroupId = null;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("'group.id' configuration parameter is required and was not specified.", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_GroupInstanceId()
    {
        var parameterLooper = new[] { string.Empty, null, "foo", "csharp-group-1" };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.GroupInstanceId = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Avro_HeartbeatIntervalMs()
    {
        options.HeartbeatIntervalMs = 3000;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public void Kafka_Consume_Avro_HeartbeatIntervalMs_0()
    {
        options.HeartbeatIntervalMs = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"heartbeat.interval.ms\" value 0 is outside allowed range 1..3600000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_IsolationLevels()
    {
        var parameterLooper = new[] { IsolationLevels.ReadCommitted, IsolationLevels.ReadUncommitted };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.IsolationLevel = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Avro_ReconnectBackoffMaxMs()
    {
        options.ReconnectBackoffMaxMs = 10000;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public void Kafka_Consume_Avro_ReconnectBackoffMaxMs_0()
    {
        options.ReconnectBackoffMaxMs = 0;

        var ex = ClassicAssert.Throws<InvalidOperationException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("`reconnect.backoff.max.ms` must be >= `reconnect.max.ms`", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Avro_ReconnectBackoffMs()
    {
        var parameterLooper = new[] { 0, 100 };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            options.ReconnectBackoffMs = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_SaslMechanisms_Plain()
    {
        sasl.SaslMechanism = SaslMechanisms.Plain;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Avro_SaslMechanisms_Gssapi()
    {
        sasl.SaslMechanism = SaslMechanisms.Gssapi;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Avro_SaslMechanisms_ScramSha512()
    {
        sasl.SaslMechanism = SaslMechanisms.ScramSha512;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Avro_SaslMechanisms_ScramSha256()
    {
        sasl.SaslMechanism = SaslMechanisms.ScramSha256;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Avro_SaslMechanisms_OAuthBearer()
    {
        sasl.SaslMechanism = SaslMechanisms.OAuthBearer;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // There's no error if invalid creds.
    [Test]
    public void Kafka_Consume_Avro_SaslUsername_Invalid()
    {
        sasl.SaslUsername = "foo";

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // There's no error if invalid creds.
    [Test]
    public void Kafka_Consume_Avro_SaslPassword_Invalid()
    {
        sasl.SaslPassword = "foo";

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public async Task Kafka_Consume_Avro_SaslOauthbearerMethods()
    {
        var parameterLooper = new[] { SaslOauthbearerMethods.Default, SaslOauthbearerMethods.Oidc };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            sasl.SaslOauthbearerMethod = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_SaslOauthbearerClientId_Oidc()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
        sasl.SaslOauthbearerClientId = "foo";

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_SaslOauthbearerClientSecret_Oidc()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
        sasl.SaslOauthbearerClientSecret = "foo";

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_Oidc_Secret_ID_Null()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
        sasl.SaslOauthbearerClientSecret = null;
        sasl.SaslOauthbearerClientId = null;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_Oidc_SaslOauthbearerTokenEndpointUrl()
    {
        var parameterLooper = new[] { null, "localhost", string.Empty };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            sasl.SaslOauthbearerTokenEndpointUrl = item;

            var result = Kafka.Consume(input, Conn(), options, default);
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
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            sasl.SaslOauthbearerExtensions = item;

            var result = Kafka.Consume(input, Conn(), options, default);
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
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            sasl.SaslOauthbearerScope = item;

            var result = Kafka.Consume(input, Conn(), options, default);
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
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            sasl.SaslOauthbearerConfig = item;

            var result = Kafka.Consume(input, Conn(), options, default);
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
            socket.SocketKeepaliveEnable = item;
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

            var result = Kafka.Consume(input, Conn(), options, default);
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
            socket.SocketNagleDisable = item;
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"looper: {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Avro_Socket_SocketTimeoutMs()
    {
        socket.SocketTimeoutMs = 10;
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_Socket_SocketConnectionSetupTimeoutMs()
    {
        await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
        socket.SocketConnectionSetupTimeoutMs = 1000;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"));
    }

    [Test]
    public async Task Kafka_Consume_Avro_Socket_SocketMaxFails()
    {
        var parameterLooper = new[] { 0, 1 };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);
            socket.SocketMaxFails = item;
            var result = Kafka.Consume(input, Conn(), options, default);
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
            socket.SocketReceiveBufferBytes = item;
            await ProduceTestMessage(isBasic: false, msgKey: null, msg: null, records: records, schema: schema);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Data[0].Value.ToString().Contains("record"), $"looper: {item}");
        }
    }
}
