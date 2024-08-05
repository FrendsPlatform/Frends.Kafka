using System.ComponentModel;

namespace Frends.Kafka.Consume.Definitions;

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
    /// Protocol used to communicate with brokers.
    /// </summary>
    /// <example>SecurityProtocols.Plaintext</example>
    [DefaultValue(SecurityProtocols.Plaintext)]
    public SecurityProtocols SecurityProtocol { get; set; }

    /// <summary>
    /// Amount of consumed messages before ending this task.
    /// 0 = unlimited, consume until timeout or task cancellation.
    /// </summary>
    /// <example>10</example>
    public int MessageCount { get; set; }

    /// <summary>
    /// Consume operation timeout (value in ms).
    /// 0=unlimited. See other timeout options in Options-tab.
    /// </summary>
    /// <example>60000</example>
    [DefaultValue(0)]
    public int Timeout { get; set; }

    /// <summary>
    /// Set Kafka partition.
    /// Consume from all topic's partitions if set to -1.
    /// </summary>
    /// <example>10</example>
    public int Partition { get; set; }
}