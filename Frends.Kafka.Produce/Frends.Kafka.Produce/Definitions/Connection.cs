namespace Frends.Kafka.Produce.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// Socket parameters.
    /// </summary>
    /// <example>Socket { int SocketTimeoutMs }</example>
    public Socket Socket { get; set; } = new();

    /// <summary>
    /// SASL parameters.
    /// </summary>
    /// <example>Sasl { bool UseSasl }</example>
    public Sasl Sasl { get; set; } = new();

    /// <summary>
    /// SSL parameters.
    /// </summary>
    /// <example>Ssl { bool UseSsl }</example>
    public Ssl Ssl { get; set; } = new();

    /// <summary>
    /// Schema registry parameters.
    /// </summary>
    /// <example>SchemaRegistry { bool UseSchemaRegistry }</example>
    public SchemaRegistry SchemaRegistry { get; set; } = new();
}
