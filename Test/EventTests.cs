namespace Test;

public class EventTests : IClassFixture<TestFixture>
{
    public readonly HttpClient _publisherClient;

    public EventTests(TestFixture fixture)
    {
        _publisherClient = fixture.PublisherClient;
    }

    [Fact]
    public async Task Published_Events_Are_Received_By_Subscribers()
    {
        var res = await _publisherClient.GetStringAsync("/event/AAAA");

        res.Should().Be("\"events published!\"");

        while (TestEventHandlerA.Received.Count + TestEventHandlerB.Received.Count < 10)
        {
            await Task.Delay(100);
        }

        (TestEventHandlerA.Received.Count + TestEventHandlerB.Received.Count).Should().Be(10);
        TestEventHandlerA.Received.Should().AllSatisfy(x => x.ReceivedBy.Should().Be("SUBSCRIBER_A"));
        TestEventHandlerB.Received.Should().AllSatisfy(x => x.ReceivedBy.Should().Be("SUBSCRIBER_B"));

        TestEventHandlerA.Received
            .Select(r => r.Id)
            .Intersect(TestEventHandlerB.Received.Select(r => r.Id))
            .Any().Should().BeFalse();
    }
}