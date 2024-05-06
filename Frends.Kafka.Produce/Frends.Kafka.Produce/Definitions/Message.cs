using System.ComponentModel;

namespace Frends.Kafka.Produce.Definitions
{
    public class Message
    {
        public SerializerOptions KeySerializer { get; set; }

        /// <summary>
        /// Message key value.
        /// Can be empty.
        /// </summary>
        /// <example>examplekey</example>
        public dynamic Key { get; set; }

        public SerializerOptions ValueSerializer { get; set; }

        /// <summary>
        /// Message.
        /// Can be empty.
        /// </summary>
        /// <example>Example message.</example>
        public dynamic Value { get; set; }

        /// <summary>
        /// The partition value.
        /// </summary>
        /// <example>0</example>
        [DefaultValue(0)]
        public int Partition { get; set; }
    }
}