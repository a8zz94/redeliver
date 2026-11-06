using System.Net;
using Redeliver.Api.Features.DeliverEvents;

namespace Redeliver.UnitTests.Features.DeliverEvents;

public class DeliveryWorkerTests
{
    [Test]
    public async Task Sends_a_queued_event_to_the_configured_receiver()
    {
        var handler = new SignallingHandler();
        using var client = new HttpClient(handler);
        var queue = new EventQueue();
        var receiverUrl = new Uri("http://receiver.test/hooks");
        using var worker = new DeliveryWorker(queue, new EventSender(client), new ReceiverOptions(receiverUrl));

        await worker.StartAsync(CancellationToken.None);
        queue.Enqueue("""{"id":"event-1"}""");

        var delivered = await handler.Request.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await worker.StopAsync(CancellationToken.None);

        Assert.That(delivered, Is.EqualTo(receiverUrl));
    }

    private sealed class SignallingHandler : HttpMessageHandler
    {
        public TaskCompletionSource<Uri> Request { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Request.TrySetResult(request.RequestUri!);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
