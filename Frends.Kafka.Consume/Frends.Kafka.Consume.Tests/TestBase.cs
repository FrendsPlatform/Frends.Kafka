using dotenv.net;
using NUnit.Framework;

namespace Frends.Kafka.Consume.Tests;

[SetUpFixture]
public class TestConfig
{
    [OneTimeSetUp]
    public static void SetUp()
    {
        //load envs
        var root = Directory.GetCurrentDirectory();
        string projDir = Directory.GetParent(root).Parent.Parent.FullName;
        DotEnv.Load(
            options: new DotEnvOptions(
                envFilePaths: new[] { $"{projDir}{Path.DirectorySeparatorChar}.env.local" }
            )
        );
    }
}