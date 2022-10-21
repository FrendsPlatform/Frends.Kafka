using Microsoft.VisualStudio.TestTools.UnitTesting;
using Frends.Kafka.Produce.Definitions;

namespace Frends.Kafka.Produce.Tests;

[TestClass]
public class UnitTests
{
    /*
        Docker compose:
        Run command 'docker-compose up -d' in .\Frends.Kafka.Produce.Tests\Files\ 
        
        Read message(s) from topic:
        docker exec --interactive --tty broker kafka-console-consumer --bootstrap-server localhost:9092 --topic TestTopic --from-beginning
    */

    private readonly string? _hostPlaintext = "localhost:9092";
    private readonly string? _message = $"Hello {DateTime.Now}";
    private readonly string? _topic = "TestTopic";

    readonly Sasl? _sasl = new() { UseSasl = false };
    readonly Ssl? _ssl = new() { UseSSL = false };

    [TestMethod]
    public async Task Kafka_Product_Test()
    {
        var _input = new Input()
        {
            Message = _message,
            Host = _hostPlaintext,
            Topic = _topic,
        };

        var _options = new Options()
        {
            MessageTimeoutMs = default,
            Acks = default,
            MessageSendMaxRetries = default,
            ApiVersionRequest = default,
            EnableIdempotence = default,
            LingerMs = default,
            MaxInFlight = default,
            MessageMaxBytes = default,
            Partitioner = default,
            QueueBufferingMaxKbytes = default,
            QueueBufferingMaxMessages = default,
            TransactionalId = default,
            TransactionTimeoutMs = default,
        };

        var _socket = new Socket()
        {
            SocketConnectionSetupTimeoutMs = default,
            SocketKeepaliveEnable = default,
            SocketMaxFails = default,
            SocketNagleDisable = default,
            SocketProduceBufferBytes = default,
            SocketReceiveBufferBytes = default,
            SocketTimeoutMs = default,
        };

        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, default);
        Assert.IsTrue(result.Success.Equals(true) && result.Message.Equals(_message) && result.Timestamp != null);
    }

    [TestMethod]
    public async Task Kafka_ErrorHandling_Test()
    {
        var _input = new Input()
        {
            Message = _message,
            Host = _hostPlaintext,
            Topic = _topic,
        };

        var _options = new Options()
        {
            MessageTimeoutMs = 10,
            Acks = default,
            MessageSendMaxRetries = default,
            ApiVersionRequest = default,
            EnableIdempotence = default,
            LingerMs = default,
            MaxInFlight = default,
            MessageMaxBytes = default,
            Partitioner = default,
            QueueBufferingMaxKbytes = default,
            QueueBufferingMaxMessages = default,
            TransactionalId = default,
            TransactionTimeoutMs = default,
        };

        var _socket = new Socket()
        {
            SocketConnectionSetupTimeoutMs = default,
            SocketKeepaliveEnable = default,
            SocketMaxFails = default,
            SocketNagleDisable = default,
            SocketProduceBufferBytes = default,
            SocketReceiveBufferBytes = default,
            SocketTimeoutMs = default,
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => Kafka.Produce(_input, _options, _socket, _sasl, _ssl, default));
        Assert.IsTrue(ex.Message != null);
    }
}