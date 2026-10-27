using Frends.Kafka.Consume.Definitions;
using NUnit.Framework;

namespace Frends.Kafka.Consume.Tests.HelpersTests;

[TestFixture]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.Catch<Exception>(() =>
            Kafka.Consume(DefaultInput(), new Connection(), InvalidOptions(), CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = InvalidOptions();
        options.ThrowErrorOnFailure = false;
        var result = Kafka.Consume(DefaultInput(), new Connection(), options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = InvalidOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.Throws<Exception>(() =>
            Kafka.Consume(DefaultInput(), new Connection(), options, CancellationToken.None));
        Assert.That(ex!.Message, Does.Contain(CustomErrorMessage));
    }

    private static Input DefaultInput() => new() { Host = "localhost:9092", Topic = "ExampleTopic", Partition = -1 };

    private static Options InvalidOptions() => new() { SessionTimeoutMs = 20000, MaxPollIntervalMs = 10000 };
}
