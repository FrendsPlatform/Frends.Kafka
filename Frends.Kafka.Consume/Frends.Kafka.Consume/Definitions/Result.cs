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
    public bool Success { get; private set; }

    /// <summary>
    /// Result data.
    /// </summary>
    /// <example>Object { key, value }</example>
    public List<Message> Data { get; private set; }

    internal Result(bool success, List<Message> data)
    {
        Success = success;
        Data = data;
    }
}