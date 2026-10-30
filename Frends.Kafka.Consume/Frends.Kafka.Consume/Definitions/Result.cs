using System.Collections.Generic;

namespace Frends.Kafka.Consume.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// True if messages have been consumed without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// Result data.
    /// </summary>
    /// <example>Object { key, value }</example>
    public List<Message> Data { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}
