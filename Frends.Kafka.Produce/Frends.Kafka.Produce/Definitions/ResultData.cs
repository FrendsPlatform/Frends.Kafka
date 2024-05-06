namespace Frends.Kafka.Produce.Definitions
{
    public class ResultData
    {
        /// <summary>
        /// Persistence status.
        /// NotPersisted: Message was never transmitted to the broker, or failed with an error indicating it was not written to the log. Application retry risks ordering, but not duplication.
        /// Persisted: Message was written to the log and acknowledged by the broker. Note: acks='all' should be used for this to be fully trusted in case of a broker failover.
        /// PossiblyPersisted: Message was transmitted to broker, but no acknowledgement was received. Application retry risks ordering and duplication.
        /// </summary>
        /// <example>PossiblyPersisted</example>
        public dynamic Status { get; private set; }

        ///// <summary>
        ///// The Kafka message timestamp .
        ///// </summary>
        ///// <example>2022-10-19 06:00:00</example>
        //public string Timestamp { get; private set; }
    }
}