namespace Frends.Kafka.Consume.Definitions;

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

/// <summary>
/// AutoOffsetResets options.
/// </summary>
public enum AutoOffsetResets
{
    /// <summary>
    /// Automatically reset the offset to the largest offset.
    /// </summary>
    Latest,

    /// <summary>
    /// Automatically reset the offset.
    /// </summary>
    Earliest,

    /// <summary>
    /// Trigger an error.
    /// </summary>
    Error
}

/// <summary>
/// BrokerAddressFamily options.
/// </summary>
public enum BrokerAddressFamilys
{
#pragma warning disable CS1591 // self explanatory
    Any,
    V4,
    V6
#pragma warning restore CS1591 // self explanatory
}

/// <summary>
/// IsolationLevels options.
/// </summary>
public enum IsolationLevels
{
    /// <summary>
    /// Return all messages, even transactional messages which have been aborted.
    /// </summary>
    ReadUncommitted,

    /// <summary>
    /// Only return transactional messages which have been committed.
    /// </summary>
    ReadCommitted
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
    SaslInherit
}