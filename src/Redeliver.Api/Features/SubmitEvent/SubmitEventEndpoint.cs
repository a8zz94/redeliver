using Redeliver.Api.Features.DeliverEvents;

namespace Redeliver.Api.Features.SubmitEvent;

public static class SubmitEventEndpoint
{
    public static void MapSubmitEvent(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/v1/events", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(HttpRequest request, EventQueue queue)
    {
        using var reader = new StreamReader(request.Body);
        var body = await reader.ReadToEndAsync();

        queue.Enqueue(body);

        return Results.Accepted();
    }
}
