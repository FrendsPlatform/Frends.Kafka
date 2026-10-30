namespace Frends.Kafka.Consume.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// Socket parameters.
    /// </summary>
    /// <example>Socket { SocketTimeoutMs = 60000 }</example>
    public Socket Socket { get; set; } = new();

    /// <summary>
    /// SASL parameters.
    /// </summary>
    /// <example>Sasl { UseSasl = true, SaslUsername = "user" }</example>
    public Sasl Sasl { get; set; } = new();

    /// <summary>
    /// SSL parameters.
    /// </summary>
    /// <example>Ssl { UseSsl = true }</example>
    public Ssl Ssl { get; set; } = new();

    /// <summary>
    /// Schema Registry parameters.
    /// </summary>
    /// <example>SchemaRegistry { UseSchemaRegistry = true, SchemaRegistryUrl = "https://registry.example.com" }</example>
    public SchemaRegistry SchemaRegistry { get; set; } = new();
}
