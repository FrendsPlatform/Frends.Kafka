using dotenv.net;
using Frends.Kafka.Consume.Definitions;
using NUnit.Framework;

namespace Frends.Kafka.Consume.Tests;

/*
    Using cloud Confluent Kafka. Set your own configs in .env file.

    Basic testing with docker:
    Run command 'docker-compose up -d' in .\Frends.Kafka.Consume.Tests

    Read message(s) from topic:
    docker exec --interactive --tty "container's name" kafka-console-consumer --bootstrap-server localhost:9092 --topic ConsumeTopic --from-beginning
*/
public abstract class TestBase
{
    // Environment is loaded and read only once for all derived test classes.
    protected static readonly string? BootstrapServers;
    protected static readonly string? ApiKey;
    protected static readonly string? ApiKeySecret;
    protected static readonly string? SchemaRegistryUrl;
    protected static readonly string? SchemaRegistryApiKey;
    protected static readonly string? SchemaRegistryApiKeySecret;

    protected readonly string message = $"Hello {DateTime.Now}";

    protected Input input = new();
    protected Options options = new();
    protected Sasl sasl = new();
    protected Socket socket = new();
    protected Ssl ssl = new();
    protected SchemaRegistry schemaRegistry = new();

    static TestBase()
    {
        DotEnv.Load();
        BootstrapServers = Environment.GetEnvironmentVariable("ConfluentKafka_BootstrapServers");
        ApiKey = Environment.GetEnvironmentVariable("ConfluentKafka_APIKey");
        ApiKeySecret = Environment.GetEnvironmentVariable("ConfluentKafka_APIKeySecret");
        SchemaRegistryUrl = Environment.GetEnvironmentVariable("ConfluentKafka_SchemaRegistryURL");
        SchemaRegistryApiKey = Environment.GetEnvironmentVariable("ConfluentKafka_SchemaRegistryAPIKey");
        SchemaRegistryApiKeySecret = Environment.GetEnvironmentVariable("ConfluentKafka_SchemaRegistryAPIKeySecret");
    }

    protected abstract string Topic { get; }

    protected abstract bool UseSchemaRegistry { get; }

    [SetUp]
    public void Setup()
    {
        input = new Input()
        {
            Host = BootstrapServers,
            Topic = Topic,
            SecurityProtocol = SecurityProtocols.SaslSsl,
            MessageCount = 1,
            Timeout = 10000,
            Partition = -1,
        };

        options = new Options()
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

        schemaRegistry = new SchemaRegistry()
        {
            UseSchemaRegistry = UseSchemaRegistry,
            BasicAuthCredentialsSource = AuthCredentialsSources.UserInfo,
            BasicAuthUserInfo = $"{SchemaRegistryApiKey}:{SchemaRegistryApiKeySecret}",
            MaxCachedSchemas = 1000,
            RequestTimeoutMs = 30000,
            SchemaRegistryUrl = SchemaRegistryUrl,
            SslCaLocation = null,
            SslKeystoreLocation = null,
            SslKeystorePassword = null,
        };

        socket = new Socket()
        {
            SocketTimeoutMs = 60000,
            SocketConnectionSetupTimeoutMs = 30000,
            SocketKeepaliveEnable = false,
            SocketMaxFails = 1,
            SocketNagleDisable = false,
            SocketReceiveBufferBytes = 0,
        };

        // SASL is required for cloud tests.
        sasl = new Sasl()
        {
            UseSasl = true,
            SaslMechanism = SaslMechanisms.Plain,
            SaslUsername = ApiKey,
            SaslPassword = ApiKeySecret,
        };

        ssl = new Ssl()
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

    protected async Task ProduceTestMessage(bool isBasic, string? msgKey, string? msg, string? records, string? schema)
    {
        // Producer must not use the socket settings under test, otherwise broken values (e.g. 1 byte receive buffer) make ProduceAsync wait for minutes.
        var producerSocket = new Socket()
        {
            SocketTimeoutMs = 60000,
            SocketConnectionSetupTimeoutMs = 30000,
            SocketKeepaliveEnable = false,
            SocketMaxFails = 1,
            SocketNagleDisable = false,
            SocketReceiveBufferBytes = 0,
        };

        if (isBasic)
            await MessageProducer.ProduceBasic(input, options, producerSocket, sasl, ssl, msgKey, msg, default);
        else
            await MessageProducer.ProduceAvro(input, options, producerSocket, sasl, ssl, records, schema, msgKey, schemaRegistry, default);
    }

    protected Connection Conn() => new() { Socket = socket, Sasl = sasl, Ssl = ssl, SchemaRegistry = schemaRegistry };
}
