namespace Test;

public class EventTests : IClassFixture<TestFixture>
{
    public readonly HttpClient _publisherClient;

    public EventTests(TestFixture fixture)
    {
        _publisherClient = fixture.PublisherClient;
    }

    [Fact]
    public async Task Published_Events_Are_Received_By_Subscriber()
    {
        var res = await _publisherClient.GetStringAsync("/event/AAAA");

        res.Should().Be("\"events published!\"");

        while (TestEventHandlerA.Received.Count < 5 && TestEventHandlerB.Received.Count < 5)
        {
            await Task.Delay(100);
        }

        TestEventHandlerA.Received.Count.Should().Be(5);
        TestEventHandlerA.Received.Should().AllSatisfy(x => x.ReceivedBy.Should().Be("SUBSCRIBER_A"));
        TestEventHandlerA.Received.Select(r => r.Id).Except(new[] { 2, 4, 6, 8, 10 }).Any().Should().BeFalse();

        TestEventHandlerB.Received.Count.Should().Be(5);
        TestEventHandlerB.Received.Should().AllSatisfy(x => x.ReceivedBy.Should().Be("SUBSCRIBER_B"));
        TestEventHandlerB.Received.Select(r => r.Id).Except(new[] { 1, 3, 5, 7, 9 }).Any().Should().BeFalse();
    }
}