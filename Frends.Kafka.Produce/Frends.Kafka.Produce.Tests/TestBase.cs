using Frends.Kafka.Produce.Definitions;

namespace Frends.Kafka.Produce.Tests;

internal abstract class TestBase
{
    protected static Input DefaultInput() => new()
    {
        Host = "localhost:9092",
        Topic = "ExampleTopic",
        Message = "Example message.",
        Partition = -1,
    };

    protected static Connection DefaultConnection() => new()
    {
        SchemaRegistry = new SchemaRegistry
        {
            UseSchemaRegistry = true,
            SchemaRegistryUrl = "http://localhost:8081",
            SchemaJson = "this is not a valid avro schema",
            Records = "{}",
        },
    };

    protected static Options DefaultOptions() => new();
}
