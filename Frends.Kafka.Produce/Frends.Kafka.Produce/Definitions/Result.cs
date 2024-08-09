namespace Frends.Kafka.Produce.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// True if message was produced successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Produce result.
    /// </summary>
    /// <example>PossiblyPersisted</example>
    public dynamic Data { get; private set; }

    internal Result(bool success, dynamic data)
    {
        Success = success;
        Data = data;
    }
}