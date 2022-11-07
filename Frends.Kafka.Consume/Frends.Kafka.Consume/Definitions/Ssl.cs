using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Kafka.Consume.Definitions;

/// <summary>
/// SSL parameters.
/// </summary>
public class Ssl
{
    /// <summary>
    /// Use SSL.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool UseSsl { get; set; }

    /// <summary>
    /// Endpoint identification algorithm to validate broker hostname using broker certificate.
    /// https - Server (broker) hostname verification as specified in RFC2818. 
    /// none - No endpoint verification. 
    /// OpenSSL >= 1.0.2 required.
    /// </summary>
    /// <example>SslEndpointIdentificationAlgorithm.None</example>
    [UIHint(nameof(UseSsl), "", true)]
    [DefaultValue(SslEndpointIdentificationAlgorithms.None)]
    public SslEndpointIdentificationAlgorithms SslEndpointIdentificationAlgorithm { get; set; }

    /// <summary>
    /// Enable OpenSSL's builtin broker (server) certificate verification.
    /// </summary>
    /// <example>true</example>
    [UIHint(nameof(UseSsl), "", true)]
    [DefaultValue(true)]
    public bool EnableSslCertificateVerification { get; set; }

    /// <summary>
    /// Path to client's public key (PEM) used for authentication.
    /// </summary>
    /// <example>c:\temp</example>
    [UIHint(nameof(UseSsl), "", true)]
    public string SslCertificateLocation { get; set; } = "";

    /// <summary>
    /// File or directory path to CA certificate(s) for verifying the broker's key.
    /// </summary>
    /// <example>Root</example>
    [UIHint(nameof(UseSsl), "", true)]
    public string SslCaLocation { get; set; } = "";

    /// <summary>
    /// Path to client's private key (PEM) used for authentication.
    /// </summary>
    /// <example>c:\temp</example>
    [UIHint(nameof(UseSsl), "", true)]
    public string SslKeyLocation { get; set; } = "";

    /// <summary>
    /// Path to client's keystore (PKCS#12) used for authentication.
    /// </summary>
    /// <example>c:\temp</example>
    [UIHint(nameof(UseSsl), "", true)]
    public string SslKeystoreLocation { get; set; } = "";

    /// <summary>
    /// Path to OpenSSL engine library. 
    /// OpenSSL >= 1.1.0 required.
    /// </summary>
    /// <example>c:\temp</example>
    [UIHint(nameof(UseSsl), "", true)]
    public string SslEngineLocation { get; set; } = "";

    /// <summary>
    /// Path to CRL for verifying broker's certificate validity.
    /// </summary>
    /// <example>c:\temp</example>
    [UIHint(nameof(UseSsl), "", true)]
    public string SslCrlLocation { get; set; } = "";

    /// <summary>
    /// Client's public key string (PEM format) used for authentication.
    /// </summary>
    /// <example>—–BEGIN PRIVATE KEY—–MIIES42Cg6zn—–END PRIVATE KEY—–</example>
    [UIHint(nameof(UseSsl), "", true)]
    [PasswordPropertyText]
    public string SslCertificatePem { get; set; } = "";

    /// <summary>
    /// A certificate string (PEM format) for verifying the broker's key.
    /// </summary>
    /// <example>—–BEGIN PRIVATE KEY—–MIIES42Cg6zn—–END PRIVATE KEY—–</example>
    [UIHint(nameof(UseSsl), "", true)]
    [PasswordPropertyText]
    public string SslCaPem { get; set; } = "";

    /// <summary>
    /// Client's private key string (PEM format) used for authentication.
    /// </summary>
    /// <example>—–BEGIN PRIVATE KEY—–MIIES42Cg6zn—–END PRIVATE KEY—–</example>
    [UIHint(nameof(UseSsl), "", true)]
    [PasswordPropertyText]
    public string SslKeyPem { get; set; } = "";

    /// <summary>
    /// Comma-separated list of Windows Certificate stores to load CA certificates from. 
    /// Certificates will be loaded in the same order as stores are specified.
    /// If no certificates can be loaded from any of the specified stores an error is logged and the OpenSSL library's default CA location is used instead. 
    /// </summary>
    /// <example>Root</example>
    [UIHint(nameof(UseSsl), "", true)]
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("Root")]
    public string SslCaCertificateStores { get; set; }

    /// <summary>
    /// Client's keystore (PKCS#12) password.
    /// </summary>
    /// <example>ExamplePassword</example>
    [UIHint(nameof(UseSsl), "", true)]
    [PasswordPropertyText]
    public string SslKeystorePassword { get; set; } = "";

    /// <summary>
    /// Private key passphrase for use with Ssl.SslKeyLocation)
    /// </summary>
    /// <example>ExamplePassword</example>
    [UIHint(nameof(UseSsl), "", true)]
    [PasswordPropertyText]
    public string SslKeyPassword { get; set; } = "";

    /// <summary>
    /// A cipher suite is a named combination of authentication, encryption, MAC and key exchange algorithm used to negotiate the security settings for a network connection using TLS or SSL network protocol.
    /// </summary>
    /// <example>foo</example>
    [UIHint(nameof(UseSsl), "", true)]
    public string SslCipherSuites { get; set; } = "";

    /// <summary>
    /// The supported-curves extension in the TLS ClientHello message specifies the curves (standard/named, or 'explicit' GF(2^k) or GF(p)) the client is willing to have the server use.
    /// </summary>
    /// <example>1</example>
    [UIHint(nameof(UseSsl), "", true)]
    public string SslCurvesList { get; set; } = "";

    /// <summary>
    /// The client uses the TLS ClientHello signature_algorithms extension to indicate to the server which signature/hash algorithm pairs may be used in digital signatures.
    /// </summary>
    /// <example>1</example>
    [UIHint(nameof(UseSsl), "", true)]
    public string SslSigalgsList { get; set; } = "";
}