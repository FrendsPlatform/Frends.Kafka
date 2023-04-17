using NUnit.Framework;
using Frends.Kafka.Consume.Definitions;
using Confluent.Kafka;
using System.Text.Json;
using System.Runtime.InteropServices;

namespace Frends.Kafka.Consume.Tests;

[TestFixture]
public class UnitTests
{
    /*
        Docker compose:
        Run command 'docker-compose up -d' in .\Frends.Kafka.Consume.Tests\Files\ 
        
        Read message(s) from topic:
        docker exec --interactive --tty "container's name" kafka-console-consumer --bootstrap-server localhost:9092 --topic ConsumeTopic --from-beginning
    */

    private readonly string _hostPlaintext = "localhost:9092";
    private readonly string _hostSaslPlaintext = "localhost:9093";
    private readonly string _username = "admin";
    private readonly string _password = "admin-secret";
    private readonly string _message = $"Hello {DateTime.Now}";
    private readonly string _topic = "ConsumeTopic";

    readonly Sasl? _sasl = new() { UseSasl = false };
    readonly Ssl? _ssl = new() { UseSsl = false };

    private static Input _input = new();
    private static Options _options = new();
    private static Socket _socket = new();

    [SetUp]
    public void Setup()
    {
        _input = new Input()
        {
            Host = _hostPlaintext,
            Topic = _topic,
            SecurityProtocol = SecurityProtocols.Plaintext,
            MessageCount = 10,
            Timeout = 10000,
        };

        _options = new Options()
        {
            Acks = Ack.None,
            MaxInFlight = 1000000,
            MessageMaxBytes = 1000000,
            ConnectionsMaxIdleMs = 10000,
            AllowAutoCreateTopics = false,
            ApiVersionFallbackMs = 0,
            ApiVersionRequest = true,
            ApiVersionRequestTimeoutMs = 10000,
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
            GroupId = null,
            GroupInstanceId = null,
            HeartbeatIntervalMs = 3000,
            IsolationLevel = IsolationLevels.ReadUncommitted,
            QueuedMaxMessagesKbytes = 65536,
            QueuedMinMessages = 1,
            ReconnectBackoffMaxMs = 10000,
            ReconnectBackoffMs = 100,
            SessionTimeoutMs = 10000,
            MaxPollIntervalMs = 20000,
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
    }

    [TearDown]
    public void Teardown()
    {
        _input = new Input()
        {
            Host = _hostPlaintext,
            Topic = _topic,
            SecurityProtocol = SecurityProtocols.Plaintext,
            MessageCount = 10,
            Timeout = 10000,
        };

        _options = new Options()
        {
            Acks = Ack.None,
            MaxInFlight = 1000000,
            MessageMaxBytes = 1000000,
            ConnectionsMaxIdleMs = 10000,
            AllowAutoCreateTopics = false,
            ApiVersionFallbackMs = 0,
            ApiVersionRequest = true,
            ApiVersionRequestTimeoutMs = 10000,
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
            GroupId = null,
            GroupInstanceId = null,
            HeartbeatIntervalMs = 3000,
            IsolationLevel = IsolationLevels.ReadUncommitted,
            QueuedMaxMessagesKbytes = 65536,
            QueuedMinMessages = 1,
            ReconnectBackoffMaxMs = 10000,
            ReconnectBackoffMs = 100,
            SessionTimeoutMs = 10000,
            MaxPollIntervalMs = 20000,
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
    }

    [Test]
    public async Task Kafka_Consume_Test()
    {
        ProducerConfig config = new()
        {
            BootstrapServers = _hostPlaintext
        };
        await ProduceTestMessage(config);
        var result = Kafka.Consume(_input, _options, _socket, _sasl, _ssl, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Messages.Any(x => x.Value.Contains(_message)));
        Assert.AreEqual(2, result.Messages.Count);
    }

    [Test]
    public void Kafka_Consume_WithoutMessages_NoError_Test()
    {
        var result = Kafka.Consume(_input, _options, _socket, _sasl, _ssl, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.Messages.Count);
    }

    [Test]
    public async Task Kafka_ConsumeSaSL()
    {
        ProducerConfig config = new()
        {
            BootstrapServers = _hostSaslPlaintext,
            SecurityProtocol = SecurityProtocol.SaslPlaintext,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = _username,
            SaslPassword = _password
        };
        await ProduceTestMessage(config);

        var sasl = new Sasl
        {
            UseSasl = true,
            SaslMechanism = SaslMechanisms.Plain,
            SaslKerberosPrincipal = "",
            SaslKerberosServiceName = "",
            SaslUsername = _username,
            SaslPassword = _password,
        };

        _input.Host = _hostSaslPlaintext;
        _input.SecurityProtocol = SecurityProtocols.SaslPlaintext;

        var result = Kafka.Consume(_input, _options, _socket, sasl, _ssl, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Messages.Count);
        Assert.IsTrue(result.Messages.Any(x => x.Value.Contains(_message)));
    }

    [Test]
    public void Kafka_Consume_TestWithInvalidTimeoutProperties()
    {
        _options.MaxPollIntervalMs = 0;
        _options.SessionTimeoutMs = 60;

        var ex = Assert.Throws<Exception>(() => Kafka.Consume(_input, _options, _socket, _sasl, _ssl, default));
        Assert.AreEqual("Options.MaxPollIntervalMs must be >= Options.SessionTimeoutMs", ex.Message);
    }

    private async Task ProduceTestMessage(ProducerConfig config)
    {
        using var producer = new ProducerBuilder<Null, string>(config).Build();
        for (int i = 0; i < 2; i++)
            await producer.ProduceAsync(_topic, new Message<Null, string> { Value = JsonSerializer.Serialize(_message + i.ToString()) });
    }
}