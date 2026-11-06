using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Redeliver.AcceptanceTests.Support;

public sealed class TestReceiver : IAsyncDisposable
{
    private readonly WebApplication _app;
    private readonly ConcurrentQueue<string> _deliveries = new();

    private TestReceiver(WebApplication app)
    {
        _app = app;
        _app.MapPost("/", async (HttpRequest request) =>
        {
            using var reader = new StreamReader(request.Body);
            _deliveries.Enqueue(await reader.ReadToEndAsync());
            return Results.Ok();
        });
    }

    public IReadOnlyCollection<string> Deliveries => _deliveries.ToArray();

    public string Url => _app.Services.GetRequiredService<IServer>()
        .Features.GetRequiredFeature<IServerAddressesFeature>()
        .Addresses.Single();

    public static async Task<TestReceiver> StartAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");

        var receiver = new TestReceiver(builder.Build());
        await receiver._app.StartAsync();
        return receiver;
    }

    public async ValueTask DisposeAsync()
    {
        await _app.StopAsync();
        await _app.DisposeAsync();
    }
}
