using dotenv.net;
using NUnit.Framework;

namespace Frends.Kafka.Consume.Tests;

[SetUpFixture]
public class TestConfig
{
    [OneTimeSetUp]
    public static void SetUp()
    {
        // load envs
        DotEnv.Load();
    }
}
