using Redeliver.Api.Features.DeliverEvents;

namespace Redeliver.UnitTests.Features.DeliverEvents;

public class EventQueueTests
{
    [Test]
    public async Task Returns_events_in_the_order_they_were_added()
    {
        var queue = new EventQueue();

        queue.Enqueue("first");
        queue.Enqueue("second");

        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var received = new List<string>();

        await foreach (var body in queue.ReadAllAsync(cancellation.Token))
        {
            received.Add(body);

            if (received.Count == 2)
            {
                break;
            }
        }

        Assert.That(received, Is.EqualTo(new[] { "first", "second" }));
    }
}
