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
    /// Persistence status.
    /// </summary>
    /// <example>PossiblyPersisted</example>
    public string Status { get; private set; }

    /// <summary>
    /// The Kafka message timestamp .
    /// </summary>
    /// <example>2022-10-19 06:00:00</example>
    public string Timestamp { get; private set; }

    internal Result(bool success, string status, string timestamp)
    {
        Success = success;
        Status = status;
        Timestamp = timestamp;
    }
}