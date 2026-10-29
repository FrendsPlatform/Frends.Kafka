namespace Frends.Kafka.Produce.Definitions;

/// <summary>
/// SecurityProtocols options.
/// </summary>
public enum SecurityProtocols
{
    /// <summary>
    /// Unencrypted communication without authentication.
    /// </summary>
    Plaintext,

    /// <summary>
    /// Encrypted communication using SSL.
    /// </summary>
    Ssl,

    /// <summary>
    /// SASL authentication over an unencrypted connection.
    /// </summary>
    SaslPlaintext,

    /// <summary>
    /// SASL authentication over an SSL encrypted connection.
    /// </summary>
    SaslSsl,
}

/// <summary>
/// CompressionTypes options.
/// </summary>
public enum CompressionTypes
{
    /// <summary>
    /// No compression.
    /// </summary>
    None,

    /// <summary>
    /// Gzip compression.
    /// </summary>
    Gzip,

    /// <summary>
    /// Snappy compression.
    /// </summary>
    Snappy,

    /// <summary>
    /// LZ4 compression.
    /// </summary>
    Lz4,

    /// <summary>
    /// Zstandard compression.
    /// </summary>
    Zstd,
}

/// <summary>
/// Ack options.
/// </summary>
public enum Ack
{
    /// <summary>
    /// Broker does not Produce any response/ack to client.
    /// </summary>
    None,

    /// <summary>
    /// The leader will write the record to its local log but will respond without awaiting full acknowledgement from all followers.
    /// </summary>
    Leader,

    /// <summary>
    /// Broker will block until message is committed by all in sync replicas (ISRs).
    /// </summary>
    All,
}

/// <summary>
/// Partitioners options.
/// </summary>
public enum Partitioners
{
    /// <summary>
    /// Random distribution.
    /// </summary>
    Random,

    /// <summary>
    /// CRC32 hash of key (Empty and NULL keys are mapped to single partition).
    /// </summary>
    Consistent,

    /// <summary>
    /// CRC32 hash of key (Empty and NULL keys are randomly partitioned).
    /// </summary>
    ConsistentRandom,

    /// <summary>
    /// Java Producer compatible Murmur2 hash of key (NULL keys are mapped to single partition)
    /// </summary>
    Murmur2,

    /// <summary>
    /// Java Producer compatible Murmur2 hash of key (NULL keys are randomly partitioned. This is functionally equivalent to the default partitioner in the Java Producer.)
    /// </summary>
    Murmur2Random,
}

/// <summary>
/// SaslMechanisms options.
/// </summary>
public enum SaslMechanisms
{
    /// <summary>
    /// GSSAPI (Kerberos) mechanism.
    /// </summary>
    Gssapi,

    /// <summary>
    /// Plain username and password mechanism.
    /// </summary>
    Plain,

    /// <summary>
    /// SCRAM-SHA-256 mechanism.
    /// </summary>
    ScramSha256,

    /// <summary>
    /// SCRAM-SHA-512 mechanism.
    /// </summary>
    ScramSha512,

    /// <summary>
    /// OAUTHBEARER mechanism.
    /// </summary>
    OAuthBearer,
}

/// <summary>
/// SaslOauthbearerMethods options.
/// </summary>
public enum SaslOauthbearerMethods
{
    /// <summary>
    /// Default OAuth bearer method.
    /// </summary>
    Default,

    /// <summary>
    /// OpenID Connect OAuth bearer method.
    /// </summary>
    Oidc,
}

/// <summary>
/// SslEndpointIdentificationAlgorithm options.
/// </summary>
public enum SslEndpointIdentificationAlgorithms
{
    /// <summary>
    /// Value is not set.
    /// </summary>
    None,

    /// <summary>
    /// Server hostname is verified against the certificate using HTTPS rules.
    /// </summary>
    Https,
}

/// <summary>
/// Authentication credentials source.
/// </summary>
public enum AuthCredentialsSources
{
    /// <summary>
    /// Credentials are specified via the `schema.registry.basic.auth.user.info` config property in the form username:password. If `schema.registry.basic.auth.user.info` is not set, authentication is disabled.
    /// </summary>
    UserInfo,

    /// <summary>
    /// Credentials are specified via the `sasl.username` and `sasl.password` configuration properties.
    /// </summary>
    SaslInherit,
}