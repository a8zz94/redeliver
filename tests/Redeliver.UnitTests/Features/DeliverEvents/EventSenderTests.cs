using System.Net;
using Redeliver.Api.Features.DeliverEvents;

namespace Redeliver.UnitTests.Features.DeliverEvents;

public class EventSenderTests
{
    [Test]
    public async Task Posts_the_event_body_to_the_receiver_url()
    {
        var handler = new RecordingHandler();
        using var client = new HttpClient(handler);
        var sender = new EventSender(client);

        await sender.SendAsync(new Uri("http://receiver.test/hooks"), """{"id":"event-1"}""");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(handler.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(handler.Url, Is.EqualTo(new Uri("http://receiver.test/hooks")));
            Assert.That(handler.Body, Is.EqualTo("""{"id":"event-1"}"""));
            Assert.That(handler.ContentType, Is.EqualTo("application/json"));
        }
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public HttpMethod? Method { get; private set; }
        public Uri? Url { get; private set; }
        public string? Body { get; private set; }
        public string? ContentType { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Method = request.Method;
            Url = request.RequestUri;
            Body = await request.Content!.ReadAsStringAsync(cancellationToken);
            ContentType = request.Content.Headers.ContentType?.MediaType;
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
