namespace Redeliver.Api.Features.DeliverEvents;

public sealed class DeliveryWorker(EventQueue queue, EventSender sender, ReceiverOptions receiver) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var body in queue.ReadAllAsync(stoppingToken))
        {
            await sender.SendAsync(receiver.Url, body, stoppingToken);
        }
    }
}
