using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Kafka.Produce.Definitions;

/// <summary>
/// SASL connection parameters.
/// </summary>
public class Sasl
{
    /// <summary>
    /// Use SASL.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool UseSasl { get; set; }

    /// <summary>
    /// SASL mechanism to use for authentication.
    /// </summary>
    /// <example>SaslMechanisms.ScramSha256</example>
    [UIHint(nameof(UseSasl), "", true)]
    [DefaultValue(SaslMechanisms.ScramSha256)]
    public SaslMechanisms SaslMechanism { get; set; }

    /// <summary>
    /// SASL username for use with the PLAIN, ScramSha256 or ScramSha512 mechanism.
    /// </summary>
    /// <example>ExampleUser</example>
    [UIHint(nameof(UseSasl), "", true)]
    public string SaslUsername { get; set; } = "";

    /// <summary>
    /// SASL password for use with the PLAIN, ScramSha256 or ScramSha512 mechanism.
    /// </summary>
    /// <example>ExamplePassword</example>
    [UIHint(nameof(UseSasl), "", true)]
    [PasswordPropertyText]
    public string SaslPassword { get; set; } = "";

    /// <summary>
    /// Set to "default" or "oidc" to control which login method to be used. 
    /// If set to "oidc", the following properties must also be be specified: 
    /// Sasl.SaslOauthbearerClientId, 
    /// Sasl.SaslOauthbearerClientSecret, 
    /// Sasl.SaslOauthbearerTokenEndpointUrl.
    /// </summary>
    /// <example>SaslOauthbearerMethod.Default</example>
    [UIHint(nameof(UseSasl), "", true)]
    [DefaultValue(SaslOauthbearerMethods.Default)]
    public SaslOauthbearerMethods SaslOauthbearerMethod { get; set; }

    /// <summary>
    /// Public identifier for the application. 
    /// Must be unique across all clients that the authorization server handles. 
    /// </summary>
    /// <example>ExampleClient</example>
    [UIHint(nameof(SaslOauthbearerMethod), "", SaslOauthbearerMethods.Oidc)]
    public string SaslOauthbearerClientId { get; set; } = "";

    /// <summary>
    /// Client secret only known to the application and the authorization server. 
    /// </summary>
    /// <example>ExampleSecret</example>
    [PasswordPropertyText]
    [UIHint(nameof(SaslOauthbearerMethod), "", SaslOauthbearerMethods.Oidc)]
    public string SaslOauthbearerClientSecret { get; set; } = "";

    /// <summary>
    /// OAuth/OIDC issuer token endpoint HTTP(S) URI used to retrieve token.
    /// </summary>
    /// <example>ExampleURL</example>
    [UIHint(nameof(SaslOauthbearerMethod), "", SaslOauthbearerMethods.Oidc)]
    public string SaslOauthbearerTokenEndpointUrl { get; set; } = "";

    /// <summary>
    /// Allow additional information to be provided to the broker. 
    /// Comma-separated list of key=value pairs. E.g., "supportFeatureX=true,organizationId=sales-emea".
    /// </summary>
    /// <example>supportFeatureX=true,organizationId=sales-emea</example>
    [UIHint(nameof(SaslOauthbearerMethod), "", SaslOauthbearerMethods.Oidc)]
    public string SaslOauthbearerExtensions { get; set; } = "";

    /// <summary>
    /// Client use this to specify the scope of the access request to the broker.
    /// </summary>
    /// <example>ExampleScope</example>
    [UIHint(nameof(SaslOauthbearerMethod), "", SaslOauthbearerMethods.Oidc)]
    public string SaslOauthbearerScope { get; set; } = "";

    /// <summary>
    /// SASL/OAUTHBEARER configuration.
    /// </summary>
    /// <example>principal=admin extension_traceId=123</example>
    [UIHint(nameof(UseSasl), "", true)]
    public string SaslOauthbearerConfig { get; set; } = "";

    /// <summary>
    /// Path to Kerberos keytab file.
    /// </summary>
    /// <example>c:\temp</example>
    [UIHint(nameof(UseSasl), "", true)]
    public string SaslKerberosKeytab { get; set; } = "";

    /// <summary>
    /// Minimum time in milliseconds between key refresh attempts. 
    /// Disable automatic key refresh by setting this property to 0.
    /// </summary>
    /// <example>60000</example>
    [UIHint(nameof(UseSasl), "", true)]
    [DefaultValue(60000)]
    public int SaslKerberosMinTimeBeforeRelogin { get; set; }

    /// <summary>
    /// This client's Kerberos principal name. 
    /// (Not supported on Windows, will use the logon user's principal).
    /// </summary>
    /// <example>kafkaclient</example>
    [UIHint(nameof(UseSasl), "", true)]
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("kafkaclient")]
    public string SaslKerberosPrincipal { get; set; }

    /// <summary>
    /// Kerberos principal name that Kafka runs as, not including /hostname@REALM
    /// </summary>
    /// <example>kafka</example>
    [UIHint(nameof(UseSasl), "", true)]
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("kafka")]
    public string SaslKerberosServiceName { get; set; }
}