using Frends.Kafka.Produce.Definitions;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Frends.Kafka.Produce.Tests;

[TestFixture]
public class UnitTests
{
    /*
        Run command:

        - on linux/git bash: ./Frends.Kafka.Produce.Test/Prebuild/prebuildCommand.sh
        - on windows: .\Frends.Kafka.Produce.Tests\Prebuild\prebuildCommand.ps1
        
        Read message(s) from topic:
        docker exec --interactive --tty broker kafka-console-consumer --bootstrap-server localhost:9092 --topic TestTopic --from-beginning
    */

    private readonly string? _hostPlaintext = "localhost:9092";
    private readonly string? _message = $"Hello {DateTime.Now}";
    private readonly string? _topic = "TestTopic";

    readonly Sasl? _sasl = new() { UseSasl = false };
    readonly Ssl? _ssl = new() { UseSsl = false };

    [Test]
    public async Task Kafka_Produce_Test()
    {
        var _input = new Input()
        {
            Message = _message,
            Host = _hostPlaintext,
            Topic = _topic
        };

        var _options = new Options();
        var _socket = new Socket();

        var result = await Kafka.Produce(_input, _options, _socket, _sasl, _ssl, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(!string.IsNullOrEmpty(result.Timestamp));
    }

    [Test]
    public void Kafka_ErrorHandling_Test()
    {
        var _input = new Input()
        {
            Message = _message,
            Host = _hostPlaintext,
            Topic = _topic,
        };

        var _options = new Options()
        {
            MessageTimeoutMs = 1
        };

        var _socket = new Socket();

        var ex = Assert.ThrowsAsync<Exception>(() => Kafka.Produce(_input, _options, _socket, _sasl, _ssl, default));
        Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
    }

    [Test]
    public async Task Kafka_ProduceSSL()
    {
        var ssl = new Ssl
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

        var input = new Input()
        {
            Message = _message,
            Host = _hostPlaintext,
            Topic = _topic,
        };

        var options = new Options();
        var socket = new Socket();

        var result = await Kafka.Produce(input, options, socket, _sasl, ssl, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(!string.IsNullOrEmpty(result.Timestamp));
    }

    [Test]
    public async Task Kafka_ProduceSaSL()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            Assert.Ignore("Sasl is not supported on Windows.");
        else
        {
            var sasl = new Sasl
            {
                UseSasl = true,
                SaslMechanism = SaslMechanisms.Plain,
                SaslKerberosPrincipal = "",
                SaslKerberosServiceName = ""
            };

            var input = new Input()
            {
                Message = _message,
                Host = _hostPlaintext,
                Topic = _topic,
            };

            var options = new Options();
            var socket = new Socket();

            var result = await Kafka.Produce(input, options, socket, sasl, _ssl, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(!string.IsNullOrEmpty(result.Timestamp));
        }
    }
}