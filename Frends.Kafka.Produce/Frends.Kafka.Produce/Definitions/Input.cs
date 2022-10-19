using Confluent.Kafka;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
    /// Message.
    /// </summary>
    /// <example>Example message.</example>
    public string Message { get; set; }

    /// <summary>
    /// Topic.
    /// </summary>
    /// <example>ExampleTopic</example>
    public string Topic { get; set; }

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