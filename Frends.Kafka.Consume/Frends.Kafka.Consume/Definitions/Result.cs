using System.Collections.Generic;

namespace Frends.Kafka.Consume.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// True if messages have been consumed. 
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Message(s).
    /// </summary>
    /// <example>Object { key, value }</example>
    public List<Message> Messages { get; private set; }

    internal Result(bool success, List<Message> messages)
    {
        Success = success;
        Messages = messages;
    }
}