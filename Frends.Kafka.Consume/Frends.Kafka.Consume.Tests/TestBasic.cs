using Confluent.Kafka;
using Frends.Kafka.Consume.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Frends.Kafka.Consume.Tests;

[TestFixture]
public class TestBasic : TestBase
{
    protected override string Topic => "ConsumeBasicTopic";

    protected override bool UseSchemaRegistry => false;

    [Test]
    public async Task Kafka_Consume_Basic_Test()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    public void Kafka_Consume_Basic_InvalidTopic()
    {
        input.Topic = "invalidTopic";
        var ex = ClassicAssert.Throws<ConsumeException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.NotNull(ex);
        ClassicAssert.AreEqual("Subscribed topic not available: invalidTopic: Broker: Unknown topic or partition", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Basic_MessageCount_Random()
    {
        var rndCount = new Random().Next(1, 6);
        input.MessageCount = rndCount;

        for (int i = 0; i < rndCount; i++)
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(rndCount, result.Data.Count);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    [NUnit.Framework.Ignore("Manual test.")]
    public async Task Kafka_Consume_Basic_MessageCount_DontReadAll()
    {
        var rndCount = new Random().Next(1, 6);
        input.MessageCount = rndCount;
        var extras = new Random().Next(1, 3);

        for (int i = 0; i < rndCount + extras; i++)
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(rndCount, result.Data.Count);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));

        var result2 = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result2.Success);
        ClassicAssert.AreEqual(extras, result2.Data.Count);
        ClassicAssert.IsTrue(result2.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    public async Task Kafka_Consume_Basic_MessageCount_Unlimited()
    {
        var rndCount = new Random().Next(1, 6);
        input.MessageCount = 0;

        for (int i = 0; i < rndCount; i++)
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));

        // We assert that we get at least the amount of messages we sent, because
        // we can't know how many messages are in the topic previously.
        ClassicAssert.IsTrue(result.Data.Count >= rndCount);
    }

    [Test]
    public async Task Kafka_Consume_Basic_Timeout_Unlimited()
    {
        input.Timeout = 0;
        var rndCount = new Random().Next(1, 6);
        input.MessageCount = rndCount;

        for (int i = 0; i < rndCount; i++)
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(rndCount, result.Data.Count);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    public async Task Kafka_Consume_Basic_SecurityProtocol_SaslSsl()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        input.SecurityProtocol = SecurityProtocols.SaslSsl;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    // This test won't fail but doesn't return anything either because cloud likes SaslSsl.
    [Test]
    public async Task Kafka_Consume_Basic_SecurityProtocol_Plaintext()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        input.SecurityProtocol = SecurityProtocols.Plaintext;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // This test won't fail but doesn't return anything either because cloud likes SaslSsl.
    [Test]
    public async Task Kafka_Consume_Basic_SecurityProtocol_SSL()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        input.SecurityProtocol = SecurityProtocols.Ssl;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public async Task Kafka_Consume_Basic_Partition()
    {
        var parameterLooper = new[] { -1, 2 };

        foreach (var item in parameterLooper)
        {
            input.Partition = item;
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_Ack()
    {
        var parameterLooper = new[] { Ack.All, Ack.None, Ack.Leader };

        // Read the whole backlog, because older messages may precede the produced one.
        input.MessageCount = 0;
        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.Acks = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_AutoCommitIntervalMs()
    {
        var parameterLooper = new[] { 0, 1000 };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.AutoCommitIntervalMs = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_AutoOffsetReset()
    {
        var parameterLooper = new[] { AutoOffsetResets.Latest, AutoOffsetResets.Earliest };

        foreach (var item in parameterLooper)
        {
            options.AutoOffsetReset = item;
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            if (item != AutoOffsetResets.Latest)
                ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_BrokerAddressFamilys()
    {
        var parameterLooper = new[] { BrokerAddressFamilys.V6, BrokerAddressFamilys.V4, BrokerAddressFamilys.Any };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.BrokerAddressFamily = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");

            if (item != BrokerAddressFamilys.V6)
                ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_CheckCrcs()
    {
        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.CheckCrcs = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Basic_ConnectionsMaxIdleMs()
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
    public async Task Kafka_Consume_Basic_Debug()
    {
        var parameterLooper = new[] { string.Empty, null, "consumer,cgrp,topic,fetch", "all" };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.Debug = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_EnableAutoCommit()
    {
        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.EnableAutoCommit = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_EnableAutoOffsetStore()
    {
        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.EnableAutoOffsetStore = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_EncodeMessageKey()
    {
        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.EncodeMessageKey = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_FetchErrorBackoffMs()
    {
        var parameterLooper = new[] { 0, 1000 };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.FetchErrorBackoffMs = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_FetchMaxBytes()
    {
        options.FetchMaxBytes = 52428800;
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    public async Task Kafka_Consume_Basic_FetchMaxBytes_0()
    {
        options.FetchMaxBytes = 0;
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

        var ex = ClassicAssert.Throws<InvalidOperationException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("`fetch.max.bytes` must be >= `message.max.bytes`", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Basic_FetchMaxBytes_MessageMaxBytes_0()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        options.FetchMaxBytes = 0;
        options.MessageMaxBytes = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"message.max.bytes\" value 0 is outside allowed range 1000..1000000000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Basic_MaxInFlight()
    {
        options.MaxInFlight = 1000000;
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    public async Task Kafka_Consume_Basic_MaxInFlight_0()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        options.MaxInFlight = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"max.in.flight.requests.per.connection\" value 0 is outside allowed range 1..1000000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Basic_QueuedMaxMessagesKbytes()
    {
        options.QueuedMaxMessagesKbytes = 65536;
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    public async Task Kafka_Consume_Basic_QueuedMaxMessagesKbytes_0()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        options.QueuedMaxMessagesKbytes = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"queued.max.messages.kbytes\" value 0 is outside allowed range 1..2097151\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Basic_QueuedMinMessages()
    {
        options.QueuedMinMessages = 65536;
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    public async Task Kafka_Consume_Basic_QueuedMinMessages_0()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        options.QueuedMinMessages = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"queued.min.messages\" value 0 is outside allowed range 1..10000000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Basic_FetchMinBytes()
    {
        options.FetchMinBytes = 1;
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    public async Task Kafka_Consume_Basic_FetchMinBytes_0()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        options.FetchMinBytes = 0;

        var ex = ClassicAssert.Throws<InvalidOperationException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"fetch.min.bytes\" value is outside allowed range 1..100000000", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Basic_FetchWaitMaxMs()
    {
        var parameterLooper = new[] { 0, 500 };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.FetchWaitMaxMs = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Basic_FetchWaitMaxMs_NoMessage()
    {
        options.FetchWaitMaxMs = 500;
        options.GroupId = $"no-message-{Guid.NewGuid()}";
        options.AutoOffsetReset = AutoOffsetResets.Latest;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.AreEqual(0, result.Data.Count);
    }

    [Test]
    public async Task Kafka_Consume_Basic_GroupId()
    {
        var parameterLooper = new[] { "csharp-group-1", "foo" };

        foreach (var item in parameterLooper)
        {
            options.GroupId = item;
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");

            if (item == "csharp-group-1")
                ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_GroupInstanceId()
    {
        var parameterLooper = new[] { string.Empty, null, "foo", "csharp-group-1" };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.GroupInstanceId = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Basic_HeartbeatIntervalMs()
    {
        options.HeartbeatIntervalMs = 3000;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public void Kafka_Consume_Basic_HeartbeatIntervalMs_0()
    {
        options.HeartbeatIntervalMs = 0;

        var ex = ClassicAssert.Throws<ArgumentException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("Configuration property \"heartbeat.interval.ms\" value 0 is outside allowed range 1..3600000\n", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Basic_IsolationLevels()
    {
        var parameterLooper = new[] { IsolationLevels.ReadCommitted, IsolationLevels.ReadUncommitted };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.IsolationLevel = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public void Kafka_Consume_Basic_ReconnectBackoffMaxMs()
    {
        options.ReconnectBackoffMaxMs = 10000;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public void Kafka_Consume_Basic_ReconnectBackoffMaxMs_0()
    {
        options.ReconnectBackoffMaxMs = 0;

        var ex = ClassicAssert.Throws<InvalidOperationException>(() => Kafka.Consume(input, Conn(), options, default));
        ClassicAssert.AreEqual("`reconnect.backoff.max.ms` must be >= `reconnect.backoff.ms`", ex?.Message);
    }

    [Test]
    public async Task Kafka_Consume_Basic_ReconnectBackoffMs()
    {
        var parameterLooper = new[] { 0, 100 };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            options.ReconnectBackoffMs = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_SaslMechanisms_Plain()
    {
        sasl.SaslMechanism = SaslMechanisms.Plain;
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Basic_SaslMechanisms_Gssapi()
    {
        sasl.SaslMechanism = SaslMechanisms.Gssapi;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Basic_SaslMechanisms_ScramSha512()
    {
        sasl.SaslMechanism = SaslMechanisms.ScramSha512;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Basic_SaslMechanisms_ScramSha256()
    {
        sasl.SaslMechanism = SaslMechanisms.ScramSha256;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // Can't be tested with real data.
    [Test]
    public void Kafka_Consume_Basic_SaslMechanisms_OAuthBearer()
    {
        sasl.SaslMechanism = SaslMechanisms.OAuthBearer;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // There's no error if invalid creds.
    [Test]
    public void Kafka_Consume_Basic_SaslUsername_Invalid()
    {
        sasl.SaslUsername = "foo";

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    // There's no error if invalid creds.
    [Test]
    public void Kafka_Consume_Basic_SaslPassword_Invalid()
    {
        sasl.SaslPassword = "foo";

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
    }

    [Test]
    public async Task Kafka_Consume_Basic_SaslOauthbearerMethods()
    {
        var parameterLooper = new[] { SaslOauthbearerMethods.Default, SaslOauthbearerMethods.Oidc };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            sasl.SaslOauthbearerMethod = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_SaslOauthbearerClientId_Oidc()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
        sasl.SaslOauthbearerClientId = "foo";

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    public async Task Kafka_Consume_Basic_SaslOauthbearerClientSecret_Oidc()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
        sasl.SaslOauthbearerClientSecret = "foo";

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    public async Task Kafka_Consume_Basic_Oidc_Secret_ID_Null()
    {
        await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
        sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
        sasl.SaslOauthbearerClientSecret = null;
        sasl.SaslOauthbearerClientId = null;

        var result = Kafka.Consume(input, Conn(), options, default);
        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
    }

    [Test]
    public async Task Kafka_Consume_Basic_Oidc_SaslOauthbearerTokenEndpointUrl()
    {
        var parameterLooper = new[] { null, "localhost", string.Empty };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            sasl.SaslOauthbearerTokenEndpointUrl = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_Oidc_SaslOauthbearerExtensions()
    {
        var parameterLooper = new[] { null, "supportFeatureX=true,organizationId=sales-emea", string.Empty };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            sasl.SaslOauthbearerExtensions = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_Oidc_SaslOauthbearerScope()
    {
        var parameterLooper = new[] { null, "ExampleScope", string.Empty };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            sasl.SaslOauthbearerScope = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_Oidc_SaslOauthbearerConfig()
    {
        var parameterLooper = new[] { null, "principal=admin extension_traceId=123", string.Empty };

        foreach (var item in parameterLooper)
        {
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);
            sasl.SaslOauthbearerMethod = SaslOauthbearerMethods.Oidc;
            sasl.SaslOauthbearerConfig = item;

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success, $"parameterLooper = {item}");
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)), $"parameterLooper = {item}");
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_Socket_SocketKeepaliveEnable()
    {
        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {
            socket.SocketKeepaliveEnable = item;
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_Socket_SocketNagleDisable()
    {
        var parameterLooper = new[] { true, false };

        foreach (var item in parameterLooper)
        {
            socket.SocketNagleDisable = item;
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_Socket_SocketTimeoutMs()
    {
        var parameterLooper = new[] { 10, 60000 };

        foreach (var item in parameterLooper)
        {
            socket.SocketTimeoutMs = item;
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_Socket_SocketConnectionSetupTimeoutMs()
    {
        var parameterLooper = new[] { 1000, 30000 };

        foreach (var item in parameterLooper)
        {
            socket.SocketConnectionSetupTimeoutMs = item;
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_Socket_SocketMaxFails()
    {
        var parameterLooper = new[] { 0, 1, 2 };

        foreach (var item in parameterLooper)
        {
            socket.SocketMaxFails = item;
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
        }
    }

    [Test]
    public async Task Kafka_Consume_Basic_Socket_SocketReceiveBufferBytes()
    {
        var parameterLooper = new[] { 0, 1, 2 };

        foreach (var item in parameterLooper)
        {
            socket.SocketReceiveBufferBytes = item;
            await ProduceTestMessage(isBasic: true, msgKey: null, msg: message, records: null, schema: null);

            var result = Kafka.Consume(input, Conn(), options, default);
            ClassicAssert.IsTrue(result.Success);

            // Tiny receive buffers prevent the broker connection from delivering messages.
            if (item == 0)
                ClassicAssert.IsTrue(result.Data.Any(x => x.Value.Contains(message)));
        }
    }
}
