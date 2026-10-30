namespace Frends.Kafka.Consume.Definitions;

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
    /// Communication encrypted with SSL.
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
/// SaslMechanisms options.
/// </summary>
public enum SaslMechanisms
{
    /// <summary>
    /// GSSAPI (Kerberos) mechanism.
    /// </summary>
    Gssapi,

    /// <summary>
    /// PLAIN username and password mechanism.
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
    /// Disable endpoint identification.
    /// </summary>
    None,

    /// <summary>
    /// Verify the server hostname using HTTPS endpoint identification.
    /// </summary>
    Https,
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
    Error,
}

/// <summary>
/// BrokerAddressFamily options.
/// </summary>
public enum BrokerAddressFamilys
{
    /// <summary>
    /// Use any IP address family.
    /// </summary>
    Any,

    /// <summary>
    /// Use IPv4 only.
    /// </summary>
    V4,

    /// <summary>
    /// Use IPv6 only.
    /// </summary>
    V6,
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
    ReadCommitted,
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
