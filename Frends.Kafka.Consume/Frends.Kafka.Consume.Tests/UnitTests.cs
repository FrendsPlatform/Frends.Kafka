using Microsoft.VisualStudio.TestTools.UnitTesting;
using Frends.Kafka.Consume.Definitions;
using Confluent.Kafka;
using System.Text.Json;

namespace Frends.Kafka.Consume.Tests;

[TestClass]
public class UnitTests
{
    /*
        Docker compose:
        Run command 'docker-compose up -d' in .\Frends.Kafka.Consume.Tests\Files\ 
        
        Read message(s) from topic:
        docker exec --interactive --tty broker kafka-console-consumer --bootstrap-server localhost:9092 --topic ConsumeTopic --from-beginning
    */

    private readonly string _hostPlaintext = "localhost:9092";
    private readonly string _message = $"Hello {DateTime.Now}";
    private readonly string _topic = "ConsumeTopic";

    readonly Sasl? _sasl = new() { UseSasl = false };
    readonly Ssl? _ssl = new() { UseSsl = false };

    [TestMethod]
    public async Task Kafka_Product_Test()
    {
        await ProduceTestMessage();

        var _input = new Input()
        {
            Host = _hostPlaintext,
            Topic = _topic,
            SecurityProtocol = SecurityProtocols.Plaintext,
            MessageCount = 10,
            Timeout = 5000,
        };

        var _options = new Options()
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

        var _socket = new Socket()
        {
            SocketConnectionSetupTimeoutMs = 30000,
            SocketKeepaliveEnable = false,
            SocketMaxFails = 1,
            SocketNagleDisable = false,
            SocketReceiveBufferBytes = 0,
            SocketTimeoutMs = 10000,
        };

        var result = Kafka.Consume(_input, _options, _socket, _sasl, _ssl, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Messages.Any(x => x.Value.Contains(_message)));
        Assert.AreEqual(result.Messages.Count == 2, result.Messages.Count);
    }

    [TestMethod]
    public void Kafka_Product_WithoutMessages_NoError_Test()
    {
        var _input = new Input()
        {
            Host = _hostPlaintext,
            Topic = _topic,
            SecurityProtocol = SecurityProtocols.Plaintext,
            MessageCount = 10,
            Timeout = 5000,
        };

        var _options = new Options()
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

        var _socket = new Socket()
        {
            SocketConnectionSetupTimeoutMs = 30000,
            SocketKeepaliveEnable = false,
            SocketMaxFails = 1,
            SocketNagleDisable = false,
            SocketReceiveBufferBytes = 0,
            SocketTimeoutMs = 10000,
        };

        var result = Kafka.Consume(_input, _options, _socket, _sasl, _ssl, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(result.Messages.Count == 0, result.Messages.Count);
    }

    private async Task ProduceTestMessage()
    {
        ProducerConfig config = new() { BootstrapServers = _hostPlaintext };
        using var producer = new ProducerBuilder<Null, string>(config).Build();
        for (int i = 0; i < 2; i++) 
            await producer.ProduceAsync(_topic, new Message<Null, string> { Value = JsonSerializer.Serialize(_message + i.ToString()) });
    }
}