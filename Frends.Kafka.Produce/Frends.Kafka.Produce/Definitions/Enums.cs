namespace Frends.Kafka.Produce.Definitions;

/// <summary>
/// SecurityProtocols options.
/// </summary>
public enum SecurityProtocols
{
#pragma warning disable CS1591 // self explanatory
    Plaintext,
    Ssl,
    SaslPlaintext,
    SaslSsl
#pragma warning restore CS1591 // self explanatory
}

/// <summary>
/// CompressionTypes options.
/// </summary>
public enum CompressionTypes
{
#pragma warning disable CS1591 // self explanatory
    None,
    Gzip,
    Snappy,
    Lz4,
    Zstd
#pragma warning restore CS1591 // self explanatory
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
    All
}

/// <summary>
/// Partitioners options.
/// </summary>
public enum Partitioners
{
    /// <summary>
    /// random distribution.
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
    Murmur2Random
}

/// <summary>
/// SaslMechanisms options.
/// </summary>
public enum SaslMechanisms
{
#pragma warning disable CS1591 // self explanatory
    Gssapi,
    Plain,
    ScramSha256,
    ScramSha512,
    OAuthBearer
#pragma warning restore CS1591 // self explanatory
}

/// <summary>
/// SaslOauthbearerMethods options.
/// </summary>
public enum SaslOauthbearerMethods
{
#pragma warning disable CS1591 // self explanatory
    Default,
    Oidc
#pragma warning restore CS1591 // self explanatory
}

/// <summary>
/// SslEndpointIdentificationAlgorithm options.
/// </summary>
public enum SslEndpointIdentificationAlgorithms
{
#pragma warning disable CS1591 // self explanatory
    None,
    Https
#pragma warning restore CS1591 // self explanatory
}