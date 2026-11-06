using System.Text;

namespace Redeliver.Api.Features.DeliverEvents;

public sealed class EventSender(HttpClient client)
{
    public async Task SendAsync(Uri receiverUrl, string body, CancellationToken cancellationToken = default)
    {
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        using var response = await client.PostAsync(receiverUrl, content, cancellationToken);
    }
}
