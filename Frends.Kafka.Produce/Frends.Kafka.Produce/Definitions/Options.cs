using System.ComponentModel;

namespace Frends.Kafka.Produce.Definitions;

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
    /// When set to true, the producer will ensure that messages are successfully produced exactly once and in the original produce order. 
    /// The following configuration properties are adjusted automatically (if not modified by the user) when idempotence is enabled:
    /// Options.MaxInFlight=5 (must be less than or equal to 5), 
    /// Options.MessageProduceMaxRetries=2147483647 (must be greater than 0), 
    /// Options.Ack.All,
    /// Producer instantation will fail if user-supplied configuration is incompatible.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool EnableIdempotence { get; set; }

    /// <summary>
    /// Delay in milliseconds to wait for messages in the producer queue to accumulate before constructing message batches (MessageSets) to transmit to brokers. 
    /// A higher value allows larger and more effective (less overhead, improved compression) batches of messages to accumulate at the expense of increased message delivery latency.
    /// </summary>
    /// <example>5</example>
    [DefaultValue(5)]
    public int LingerMs { get; set; }

    /// <summary>
    /// Maximum number of in-flight requests per broker connection. 
    /// This is a generic property applied to all broker communication, however it is primarily relevant to produce requests. 
    /// In particular, note that other mechanisms limit the number of outstanding consumer fetch request per broker to one.
    /// </summary>
    /// <example>1000000</example>
    [DefaultValue(1000000)]
    public int MaxInFlight { get; set; } = 1000000;

    /// <summary>
    /// Local message timeout. This value is only enforced locally and limits the time a produced message waits for successful delivery. A time of 0 is infinite. 
    /// This is the maximum time librdkafka may use to deliver a message (including retries). 
    /// Delivery error occurs when either the retry count or the message timeout are exceeded. 
    /// The message timeout is automatically adjusted to TransactionTimeoutMs if Options.TransactionalId is configured.
    /// </summary>
    /// <example>300000</example>
    [DefaultValue(300000)]
    public int MessageTimeoutMs { get; set; }

    /// <summary>
    ///  Maximum Kafka protocol request message size. 
    ///  Due to differing framing overhead between protocol versions the producer is unable to reliably enforce a strict max message limit at produce time and may exceed the maximum size by one message in protocol ProduceRequests, the broker will enforce the the topic's Options.MessageMaxBytes limit (see Apache Kafka documentation)
    /// </summary>
    /// <example>1000000</example>
    [DefaultValue(1000000)]
    public int MessageMaxBytes { get; set; } = 1000000;

    /// <summary>
    /// How many times to retry sending a failing Message. 
    /// Retrying may cause reordering unless Options.EnableIdempotence is set to true.
    /// </summary>
    [DefaultValue(2147483647)]
    public int MessageSendMaxRetries { get; set; }

    /// <summary>
    /// Partitioner.
    /// </summary>
    /// <example>Partitioners.ConsistentRandom</example>
    [DefaultValue(Partitioners.ConsistentRandom)]
    public Partitioners Partitioner { get; set; }

    /// <summary>
    /// Maximum total message size sum allowed on the producer queue. 
    /// This queue is shared by all topics and partitions. 
    /// This property has higher priority than Options.QueueBufferingMaxMessages.
    /// </summary>
    /// <example>1048576</example>
    [DefaultValue(1048576)]
    public int QueueBufferingMaxKbytes { get; set; } = 1048576;

    /// <summary>
    /// Maximum number of messages allowed on the producer queue. This queue is shared by all topics and partitions.
    /// </summary>
    /// <example>100000</example>
    [DefaultValue(100000)]
    public int QueueBufferingMaxMessages { get; set; } = 100000;

    /// <summary>
    /// Enables the transactional producer. 
    /// Used to identify the same transactional producer instance across process restarts. 
    /// It allows the producer to guarantee that transactions corresponding to earlier instances of the same producer have been finalized prior to starting any new transactions, and that any zombie instances are fenced off. 
    /// If no TransactionalId is provided, then the producer is limited to idempotent delivery (if Options.EnableIdempotence is set). Requires broker version >= 0.11.0.
    /// </summary>
    /// <example>1</example>
    public string TransactionalId { get; set; }

    /// <summary>
    /// The maximum amount of time in milliseconds that the transaction coordinator will wait for a transaction status update from the producer before proactively aborting the ongoing transaction. 
    /// If this value is larger than the `transaction.max.timeout.ms` setting in the broker, the call will fail with Timeout error.
    /// The transaction timeout automatically adjusts Options.MessageTimeoutMs and Socket.SocketTimeoutMs, unless explicitly configured in which case they must not exceed the transaction timeout (Socket.SocketTimeoutMs must be at least 100ms lower than Options.TransactionTimeoutMs).
    /// This is also the default timeout value if no timeout (-1) is supplied to the transactional API methods.
    /// </summary>
    /// <example>60000</example>
    [DefaultValue(60000)]
    public int TransactionTimeoutMs { get; set; } = 60000;
}