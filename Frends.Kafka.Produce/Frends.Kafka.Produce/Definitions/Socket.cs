using System.ComponentModel;

namespace Frends.Kafka.Produce.Definitions;

/// <summary>
/// Socket parameters.
/// </summary>
public class Socket
{
    /// <summary>
    /// Enable TCP keep-alives (SO_KEEPALIVE) on broker sockets.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool SocketKeepaliveEnable { get; set; }

    /// <summary>
    /// Disable the Nagle algorithm (TCP_NODELAY) on broker sockets.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool SocketNagleDisable { get; set; }

    /// <summary>
    /// Default timeout for network requests. 
    /// ProduceRequests will use the lesser value of Socket.SocketTimeoutMs and remaining Options.MessageTimeoutMs for the first message in the batch.
    /// </summary>
    /// <example>60000</example>
    [DefaultValue(60000)]
    public int SocketTimeoutMs { get; set; } = 60000;

    /// <summary>
    /// Maximum time allowed for broker connection setup (TCP connection setup as well SSL and SASL handshake).
    /// If the connection to the broker is not fully functional after this the connection will be closed and retried.
    /// </summary>
    /// <example>30000</example>
    [DefaultValue(30000)]
    public int SocketConnectionSetupTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Disconnect from broker when this number of Produce failures (e.g., timed out requests) is reached. 
    /// Disable with 0. 
    /// WARNING: It is highly recommended to leave this setting at its default value of 1 to avoid the client and broker to become desynchronized in case of request timeouts. 
    /// NOTE: The connection is automatically re-established.
    /// </summary>
    /// <example>1</example>
    [DefaultValue(1)]
    public int SocketMaxFails { get; set; }

    /// <summary>
    /// Broker socket receive buffer size. 
    /// 0 = System default.
    /// </summary>
    /// <example>0</example>
    [DefaultValue(0)]
    public int SocketReceiveBufferBytes { get; set; }

    /// <summary>
    /// Broker socket Produce buffer size. 
    /// 0 = System default.
    /// </summary>
    /// <example>0</example>
    [DefaultValue(0)]
    public int SocketProduceBufferBytes { get; set; }
}