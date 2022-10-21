namespace Frends.Kafka.Produce.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Message sent.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Persistence status.
    /// </summary>
    public string Status { get; private set; }

    /// <summary>
    /// Message.
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Timestamp.
    /// </summary>
    public string Timestamp { get; private set; }

    internal Result(bool success, string status, string message, string timestamp)
    {
        Success = success;
        Status = status;
        Message = message;
        Timestamp = timestamp;
    }
}