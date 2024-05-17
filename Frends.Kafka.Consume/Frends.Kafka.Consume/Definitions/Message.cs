namespace Frends.Kafka.Consume.Definitions;

/// <summary>
/// Message's values.
/// </summary>
public class Message
{
    /// <summary>
    /// Message's key converted to string.
    /// </summary>
    /// <example>foo</example>
    public dynamic Key { get; set; }

    /// <summary>
    /// Message's value.
    /// </summary>
    /// <example>bar</example>
    public dynamic Value { get; set; }
}