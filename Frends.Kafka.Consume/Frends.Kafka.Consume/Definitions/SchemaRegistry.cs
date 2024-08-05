using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Kafka.Consume.Definitions;

/// <summary>
/// Schema registry parameters.
/// </summary>
public class SchemaRegistry
{
    /// <summary>
    /// Use Avro schema registry.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool UseSchemaRegistry { get; set; }

    /// <summary>
    /// A comma-separated list of URLs for schema registry instances that are used to register or lookup schemas.
    /// </summary>
    /// <example>http://localhost:8081</example>
    [UIHint(nameof(UseSchemaRegistry), "", true)]
    public string SchemaRegistryUrl { get; set; }

    /// <summary>
    /// Authentication credentials source.
    /// UserInfo = Credentials are specified via the BasicAuthUserInfo property in the form username:password. If BasicAuthUserInfo is not set, authentication is disabled.
    /// SaslInherit = Credentials are specified via the `Sasl.SaslUsername` and `Sasl.SaslPassword` parameters.
    /// </summary>
    /// <example>AuthCredentialsSource.UserInfo</example>
    [UIHint(nameof(UseSchemaRegistry), "", true)]
    [DefaultValue(AuthCredentialsSources.UserInfo)]
    public AuthCredentialsSources BasicAuthCredentialsSource { get; set; }

    /// <summary>
    /// Basic auth credentials in the form {username}:{password}.
    /// </summary>
    /// <example>foo:bar</example>
    [PasswordPropertyText]
    [UIHint(nameof(UseSchemaRegistry), "", true)]
    public string BasicAuthUserInfo { get; set; }

    /// <summary>
    /// Specifies the maximum number of schemas CachedSchemaRegistryClient should cache locally.
    /// </summary>
    /// <example>1000</example>
    [UIHint(nameof(UseSchemaRegistry), "", true)]
    [DefaultValue(1000)]
    public int MaxCachedSchemas { get; set; }

    /// <summary>
    /// Specifies the timeout for requests to Confluent Schema Registry.
    /// </summary>
    /// <example>30000</example>
    [UIHint(nameof(UseSchemaRegistry), "", true)]
    [DefaultValue(30000)]
    public int RequestTimeoutMs { get; set; }

    /// <summary>
    /// File path to CA certificate(s) for verifying the Schema Registry's key. 
    /// System CA certs will be used if not specified.
    /// </summary>
    /// <example>/path/to/your/file</example>
    [UIHint(nameof(UseSchemaRegistry), "", true)]
    public string SslCaLocation { get; set; }

    /// <summary>
    /// SSL keystore (PKCS#12) location.
    /// </summary>
    /// <example>/path/to/your/file</example>   
    [UIHint(nameof(UseSchemaRegistry), "", true)]
    public string SslKeystoreLocation { get; set; }

    /// <summary>
    /// SSL keystore (PKCS#12) password.
    /// </summary>
    /// <example>foo</example>
    [UIHint(nameof(UseSchemaRegistry), "", true)]
    public string SslKeystorePassword { get; set; }
}