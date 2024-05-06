using System.Collections.Generic;

namespace Frends.Kafka.Produce.Definitions
{
    public class Record
    {
        public bool IsNestedGenericRecord { get; set; }

        /// <summary>
        /// Use either RecordSchemaJson or RecordSchemaJsonFile
        /// </summary>
        public string RecordSchemaJson { get; set; }
        public string RecordSchemaJsonFile { get; set; }


        public string Name { get; set; }

        /// <summary>
        /// Use either Value or NestedRecord
        /// </summary>
        public dynamic Value { get; set; }

        public List<Record> NestedRecord { get; set; }
    }
}