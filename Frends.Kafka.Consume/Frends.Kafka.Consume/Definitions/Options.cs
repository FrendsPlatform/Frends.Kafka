using System.ComponentModel;

namespace Frends.Kafka.Consume.Definitions;

/// <summary>
/// Optional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// This field indicates the number of acknowledgements the leader broker must receive from ISR brokers before responding to the request.
    /// </summary>
    /// <example>Ack.None</example>
    [DefaultValue(Ack.None)]
    public Ack Acks { get; set; }

    /// <summary>
    /// Request broker's supported API versions to adjust functionality to available protocol features. 
    /// If set to false, or the ApiVersionRequest fails, the fallback version `broker.version.fallback` will be used. 
    /// Depends on broker version >=0.10.0. If the request is not supported by (an older) broker the `broker.version.fallback` fallback is used.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ApiVersionRequest { get; set; }

    /// <summary>
    /// Dictates how long the `broker.version.fallback` fallback is used in the case the ApiVersionRequest fails.
    /// The ApiVersionRequest is only issued when a new connection to the broker is made (such as after an upgrade).
    /// </summary>
    /// <example>0</example>
    [DefaultValue(0)]
    public int ApiVersionFallbackMs { get; set; }

    /// <summary>
    /// Timeout for broker API version requests.
    /// </summary>
    /// <example>10000</example>
    [DefaultValue(10000)]
    public int ApiVersionRequestTimeoutMs { get; set; }

    /// <summary>
    /// The frequency in milliseconds that the consumer offsets are committed (written) to offset storage. 
    /// (0 = disable). This setting is used by the high-level consumer.
    /// </summary>
    /// <example>5000</example>
    [DefaultValue(5000)]
    public int AutoCommitIntervalMs { get; set; }

    /// <summary>
    /// Allow automatic topic creation on the broker when subscribing to or assigning non-existent topics. 
    /// The broker must also be configured with `auto.create.topics.enable=true` for this configuraiton to take effect.
    /// Note: The default value (false) is different from the Java consumer (true). 
    /// Requires broker version >= 0.11.0.0, for older broker versions only the broker configuration applies.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool AllowAutoCreateTopics { get; set; }

    /// <summary>
    /// Action to take when there is no initial offset in offset store or the desired offset is out of range.
    /// </summary>
    /// <example>AutoOffsetResets.
    [DefaultValue(AutoOffsetResets.Latest)]
    public AutoOffsetResets AutoOffsetReset { get; set; }

    /// <summary>
    /// Automatically store offset of last message provided to application.
    /// The offset store is an in-memory store of the next offset to (auto-)commit for each partition.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool EnableAutoOffsetStore { get; set; }

    /// <summary>
    /// Allowed broker IP address families.
    /// </summary>
    /// <example>BrokerAddressFamilys.Any</example>
    [DefaultValue(BrokerAddressFamilys.Any)]
    public BrokerAddressFamilys BrokerAddressFamily { get; set; }

    /// <summary>
    /// Close broker connections after the specified time of inactivity. 
    /// Disable with 0. 
    /// If this property is left at its default value some heuristics are performed to determine a suitable default value, this is currently limited to identifying brokers on Azure.
    /// </summary>
    /// <example>0</example>
    [DefaultValue(0)]
    public int ConnectionsMaxIdleMs { get; set; }

    /// <summary>
    /// Verify CRC32 of consumed messages, ensuring no on-the-wire or on-disk corruption to the messages occurred. 
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool CheckCrcs { get; set; }

    /// <summary>
    /// Automatically and periodically commit offsets in the background. 
    /// Note: setting this to false does not prevent the consumer from fetching previously committed start offsets.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool EnableAutoCommit { get; set; }

    /// <summary>
    /// How long to postpone the next fetch request for a topic+partition in case of a fetch error.
    /// </summary>
    /// <example>500</example>
    [DefaultValue(500)]
    public int FetchErrorBackoffMs { get; set; }

    /// <summary>
    /// Maximum amount of data the broker shall return for a Fetch request. 
    /// Messages are fetched in batches by the consumer and if the first message batch in the first non-empty partition of the Fetch request is larger than this value, then the message batch will still be returned to ensure the consumer can make progress.
    /// The maximum message batch size accepted by the broker is defined via `message.max.bytes` (broker config) or `max.message.bytes` (broker topic config). 
    /// FetchMaxBytes is automatically adjusted upwards to be at least Options.MessageMaxBytes
    /// </summary>
    /// <example>52428800</example>
    [DefaultValue(52428800)]
    public int FetchMaxBytes { get; set; }

    /// <summary>
    /// Minimum number of bytes the broker responds with. 
    /// If option.FetchWaitMaxMs expires the accumulated data will be sent to the client regardless of this setting.
    /// </summary>
    /// <example>1</example>
    [DefaultValue(1)]
    public int FetchMinBytes { get; set; }

    /// <summary>
    /// Maximum time the broker may wait to fill the Fetch response with Options.FetchMinBytes of messages.
    /// </summary>
    /// <example>500</example>
    [DefaultValue(500)]
    public int FetchWaitMaxMs { get; set; }

    /// <summary>
    /// Client group id string. 
    /// All clients sharing the same Options.GroupId belong to the same group.
    /// </summary>
    /// <example>foo</example>
    public string GroupId { get; set; }

    /// <summary>
    /// Enable static group membership. 
    /// Static group members are able to leave and rejoin a group within the configured Options.SessionTimeoutMs without prompting a group rebalance. 
    /// This should be used in combination with a larger Options.SessionTimeoutMs to avoid group rebalances caused by transient unavailability (e.g. process restarts).
    /// Requires broker version >= 2.3.0.
    /// </summary>
    /// <example>foo</example>
    public string GroupInstanceId { get; set; }

    /// <summary>
    /// Group session keepalive heartbeat interval.
    /// </summary>
    /// <example>3000</example>
    [DefaultValue(3000)]
    public int HeartbeatIntervalMs { get; set; }

    /// <summary>
    /// Controls how to read messages written transactionally.
    /// </summary>
    /// <example>IsolationLevels.ReadCommitted</example>
    [DefaultValue(IsolationLevels.ReadCommitted)]
    public IsolationLevels IsolationLevel { get; set; }

    /// <summary>
    ///  Maximum Kafka protocol request message size. 
    ///  Due to differing framing overhead between protocol versions the producer is unable to reliably enforce a strict max message limit at produce time and may exceed the maximum size by one message in protocol ProduceRequests, the broker will enforce the the topic's Options.MessageMaxBytes limit (see Apache Kafka documentation)
    /// </summary>
    /// <example>1000000</example>
    [DefaultValue(1000000)]
    public int MessageMaxBytes { get; set; }

    /// <summary>
    /// Maximum number of in-flight requests per broker connection. 
    /// This is a generic property applied to all broker communication, however it is primarily relevant to produce requests. 
    /// In particular, note that other mechanisms limit the number of outstanding consumer fetch request per broker to one.
    /// </summary>
    /// <example>1000000</example>
    [DefaultValue(1000000)]
    public int MaxInFlight { get; set; }

    /// <summary>
    /// Maximum allowed time between calls to consume messages for high-level consumers. 
    /// If this interval is exceeded the consumer is considered failed and the group will rebalance in order to reassign the partitions to another consumer group member. 
    /// Warning: Offset commits may be not possible at this point.
    /// Options.MaxPollIntervalMs must be >= Options.SessionTimeoutMs.
    /// The interval is checked two times per second. 
    /// </summary>
    /// <example>300000</example>
    [DefaultValue(300000)]
    public int MaxPollIntervalMs { get; set; }

    /// <summary>
    /// Maximum number of kilobytes of queued pre-fetched messages in the local consumer queue.
    /// If using the high-level consumer this setting applies to the single consumer queue, regardless of the number of partitions. 
    /// When using the legacy simple consumer or when separate partition queues are used this setting applies per partition.
    /// This value may be overshot by Options.FetchMaxBytes. 
    /// This property has higher priority than Options.QueuedMinMessages.
    /// </summary>
    /// <example>65536</example>
    [DefaultValue(65536)]
    public int QueuedMaxMessagesKbytes { get; set; }

    /// <summary>
    /// Minimum number of messages per topic+partition librdkafka tries to maintain in the local consumer queue.
    /// </summary>
    /// <example>100000</example>
    [DefaultValue(100000)]
    public int QueuedMinMessages { get; set; }

    /// <summary>
    /// The maximum time to wait before reconnecting to a broker after the connection has been closed.
    /// </summary>
    /// <example>10000</example>
    [DefaultValue(10000)]
    public int ReconnectBackoffMaxMs { get; set; }

    /// <summary>
    /// The initial time to wait before reconnecting to a broker after the connection has been closed.
    /// The time is increased exponentially until Options.ReconnectBackoffMaxMs is reached. 
    /// -25% to +50% jitter is applied to each reconnect backoff. 
    /// A value of 0 disables the backoff and reconnects immediately.
    /// </summary>
    /// <example>100</example>
    [DefaultValue(100)]
    public int ReconnectBackoffMs { get; set; }

    /// <summary>
    /// Client group session and failure detection timeout. 
    /// The consumer sends periodic heartbeats (Options.HeartbeatIntervalMs) to indicate its liveness to the broker.
    /// If no hearts are received by the broker for a group member within the session timeout, the broker will remove the consumer from the group and trigger a rebalance. 
    /// Options.MaxPollIntervalMs must be >= Options.SessionTimeoutMs.
    /// </summary>
    /// <example>45000</example>
    [DefaultValue(45000)]
    public int SessionTimeoutMs { get; set; }
}