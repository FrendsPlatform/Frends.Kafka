using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
    /// Enable SSL verification. 
    /// Disabling SSL verification is insecure and should only be done for reasons of convenience in test/dev environments.
    /// </summary>
    /// <example>true</example>
    [UIHint(nameof(UseSchemaRegistry), "", true)]
    [DefaultValue(true)]
    public bool EnableSslCertificateVerification { get; set; }

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

    /// <summary>
    /// Schema JSON.
    /// </summary>
    /// <example>
    /// {
    ///   "fields": [
    ///     {
    ///       "name": "intField",
    ///       "type": "int"
    ///     },
    ///     {
    ///       "name": "longField",
    ///       "type": "long"
    ///     },
    ///     {
    ///       "name": "floatField",
    ///       "type": "float"
    ///     },
    ///     {
    ///       "name": "doubleField",
    ///       "type": "double"
    ///     },
    ///     {
    ///       "name": "booleanField",
    ///       "type": "boolean"
    ///     },
    ///     {
    ///       "name": "stringField",
    ///       "type": "string"
    ///     },
    ///     {
    ///       "name": "nullField",
    ///       "type": "null"
    ///     },
    ///     {
    ///       "name": "bytesField",
    ///       "type": "bytes"
    ///     },
    ///     {
    ///       "name": "enumField",
    ///       "type": {
    ///         "name": "Colors",
    ///         "symbols": [
    ///           "RED",
    ///           "GREEN",
    ///           "BLUE"
    ///         ],
    ///         "type": "enum"
    ///       }
    ///     },
    ///     {
    ///       "name": "arrayField",
    ///       "type": {
    ///         "items": "string",
    ///         "type": "array"
    ///       }
    ///     },
    ///     {
    ///       "name": "mapField",
    ///       "type": {
    ///         "type": "map",
    ///         "values": "int"
    ///       }
    ///     },
    ///     {
    ///       "name": "fixedField",
    ///       "type": {
    ///         "name": "FourBytes",
    ///         "size": 4,
    ///         "type": "fixed"
    ///       }
    ///     },
    ///     {
    ///       "name": "unionField",
    ///       "type": [
    ///         "null",
    ///         "string"
    ///       ]
    ///     },
    ///     {
    ///       "name": "recordField",
    ///       "type": {
    ///         "fields": [
    ///           {
    ///             "name": "nestedField",
    ///             "type": "string"
    ///           }
    ///         ],
    ///         "name": "NestedRecord",
    ///         "type": "record"
    ///       }
    ///     }
    ///   ],
    ///   "name": "sampleRecord",
    ///   "namespace": "com.mycorp.mynamespace",
    ///   "type": "record"
    /// }
    /// </example>
    [UIHint(nameof(UseSchemaRegistry), "", true)]
    public string SchemaJson { get; set; }


    /// <summary>
    /// Field values as an JSON array.
    /// </summary>
    /// <example>
    /// {
    ///   "intField": 123,
    ///   "longField": 1234567890,
    ///   "floatField": 1.23,
    ///   "doubleField": 1.23456789,
    ///   "booleanField": true,
    ///   "stringField": "Hello, World!",
    ///   "nullField": null,
    ///   "bytesField": "dGVzdDE=",
    ///   "enumField": "RED",
    ///   "arrayField": ["item1", "item2", "item3"],
    ///   "mapField": {
    ///     "key1": 1,
    ///     "key2": 2,
    ///     "key3": 3
    ///   },
    ///   "fixedField": "YWJjZA==",
    ///   "unionField": "Hello, Union!",
    ///   "recordField": {
    ///     "nestedField": "Hello, Nested!"
    ///   }
    /// }
    /// </example>
    [UIHint(nameof(UseSchemaRegistry), "", true)]
    public string Records { get; set; }
}