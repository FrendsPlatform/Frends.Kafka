using System.ComponentModel;

namespace Frends.Kafka.Produce.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Initial list of brokers as a CSV list of broker host or host:port. 
    /// </summary>
    /// <example>localhost:1234</example>
    public string Host { get; set; }

    /// <summary>
    /// Topic.
    /// </summary>
    /// <example>ExampleTopic</example>
    public string Topic { get; set; }

    /// <summary>
    /// Message key value.
    /// Can be empty.
    /// </summary>
    /// <example>examplekey</example>
    public string Key { get; set; }

    /// <summary>
    /// Message.
    /// Can be empty.
    /// </summary>
    /// <example>Example message.</example>
    public string Message { get; set; }

    /// <summary>
    /// The partition value.
    /// </summary>
    /// <example>0</example>
    [DefaultValue(0)]
    public int Partition { get; set; }

    /// <summary>
    /// Compression codec to use for compressing message sets.
    /// </summary>
    /// <example>CompressionTypes.None</example>
    [DefaultValue(CompressionTypes.None)]
    public CompressionTypes CompressionType { get; set; }

    /// <summary>
    /// Protocol used to communicate with brokers.
    /// </summary>
    /// <example>SecurityProtocols.Plaintext</example>
    [DefaultValue(SecurityProtocols.Plaintext)]
    public SecurityProtocols SecurityProtocol { get; set; }
}