using Redeliver.Api.Features.DeliverEvents;

namespace Redeliver.Api.Features.SubmitEvent;

public static class SubmitEventEndpoint
{
    public static void MapSubmitEvent(this IEndpointRouteBuilder routes) =>
        routes.MapPost("/v1/events", async (HttpRequest request, EventQueue queue) =>
        {
            using var reader = new StreamReader(request.Body);
            queue.Enqueue(await reader.ReadToEndAsync());
            return Results.Accepted();
        });
}
