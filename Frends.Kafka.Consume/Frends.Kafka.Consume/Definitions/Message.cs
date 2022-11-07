namespace Frends.Kafka.Consume.Definitions;

/// <summary>
/// Message's values.
/// </summary>
public class Message
{
    /// <summary>
    /// Message's key.
    /// </summary>
    /// <example>foo</example>
    public string Key { get; set; }

    /// <summary>
    /// Message's value.
    /// </summary>
    /// <example>bar</example>
    public string Value { get; set; }
}