using Frends.Kafka.Produce.Definitions;
using NUnit.Framework;
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
            MessageTimeoutMs = 2000,
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
        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Timestamp);
    }

    [Test]
    public async Task Kafka_Produce_Empty_Message()
    {
        _input.Message = null;
        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Timestamp);
    }

    [Test]
    public async Task Kafka_Produce_Partition_Test()
    {
        _input.Partition = 0;
        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Timestamp);
    }

    [Test]
    public async Task Kafka_Produce_Partition_Key()
    {
        _input.Key = "SomeKey";
        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Timestamp);
    }

    [Test]
    public void Kafka_ErrorHandling_Test()
    {
        _options.MessageTimeoutMs = 1;
        var ex = Assert.ThrowsAsync<Exception>(() => Kafka.Produce(_input, _options, _socket, _sasl, _ssl, default));
        Assert.IsNotNull(ex.Message);
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

        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Timestamp);
    }

    [Test]
    public async Task Kafka_ProduceSaSL()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            Assert.Ignore("Sasl is not supported on Windows.");
        else
        {
            _sasl = new Sasl
            {
                UseSasl = true,
                SaslMechanism = SaslMechanisms.Plain,
                SaslKerberosPrincipal = "",
                SaslKerberosServiceName = ""
            };

            var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, default);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Timestamp);
        }
    }
}