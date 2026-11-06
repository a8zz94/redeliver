using System.Threading.Channels;

namespace Redeliver.Api.Features.DeliverEvents;

public sealed class EventQueue
{
    private readonly Channel<string> _channel = Channel.CreateUnbounded<string>();

    public void Enqueue(string body) => _channel.Writer.TryWrite(body);

    public IAsyncEnumerable<string> ReadAllAsync(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
