using Avro;
using Confluent.SchemaRegistry;
using System.Collections.Generic;
using System.ComponentModel;

namespace Frends.Kafka.Produce.Definitions;

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
    public string SchemaRegistryUrl { get; set; }

    /// <summary>
    /// Authentication credentials source.
    /// UserInfo = Credentials are specified via the `SchemaRegistry.BasicAuthUserInfo` config property in the form username:password. If `SchemaRegistry.BasicAuthUserInfo` is not set, authentication is disabled.
    /// SaslInherit = Credentials are specified via the `Sasl.SaslUsername` and `Sasl.SaslPassword` parameters.
    /// </summary>
    /// <example>AuthCredentialsSource.UserInfo</example>
    [DefaultValue(AuthCredentialsSource.UserInfo)]
    public AuthCredentialsSources BasicAuthCredentialsSource { get; set; }

    /// <summary>
    /// Basic auth credentials in the form {username}:{password}.
    /// </summary>
    /// <example>foo:bar</example>
    public string BasicAuthUserInfo { get; set; }

    /// <summary>
    /// Enable SSL verification. 
    /// Disabling SSL verification is insecure and should only be done for reasons of convenience in test/dev environments.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool EnableSslCertificateVerification { get; set; }

    /// <summary>
    /// Specifies the maximum number of schemas CachedSchemaRegistryClient should cache locally.
    /// </summary>
    /// <example>1000</example>
    [DefaultValue(1000)]
    public int MaxCachedSchemas { get; set; }

    /// <summary>
    /// Specifies the timeout for requests to Confluent Schema Registry.
    /// </summary>
    /// <example>30000</example>
    [DefaultValue(30000)]
    public int RequestTimeoutMs { get; set; }

    /// <summary>
    /// File path to CA certificate(s) for verifying the Schema Registry's key. 
    /// System CA certs will be used if not specified.
    /// </summary>
    /// <example></example>
    public string SslCaLocation { get; set; }

    /// <summary>
    /// SSL keystore (PKCS#12) location.
    /// </summary>
    /// <example>/path/to/your/keystore.p12</example>   
    public string SslKeystoreLocation { get; set; }

    /// <summary>
    /// SSL keystore (PKCS#12) password.
    /// </summary>
    /// <example>foo</example>
    public string SslKeystorePassword { get; set; }

    /// <summary>
    /// Use either RecordSchemaJson or RecordSchemaJsonFile
    /// </summary>
    public string RecordSchemaJson { get; set; }
    public string RecordSchemaJsonFile { get; set; }

    public string Records { get; set; }
}