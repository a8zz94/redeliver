using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using Redeliver.AcceptanceTests.Support;
using Reqnroll;

namespace Redeliver.AcceptanceTests.Steps;

[Binding]
public sealed class EventDeliverySteps : IAsyncDisposable
{
    private static readonly TimeSpan DeliveryTimeout = TimeSpan.FromSeconds(10);

    private WebApplicationFactory<Program>? _api;
    private HttpClient? _client;
    private TestReceiver? _receiver;
    private HttpResponseMessage? _submission;
    private string? _eventId;

    [Given("a receiver is configured")]
    public async Task GivenAReceiverIsConfigured()
    {
        _receiver = await TestReceiver.StartAsync();

        _api = new WebApplicationFactory<Program>().WithWebHostBuilder(host =>
            host.UseSetting("Receiver:Url", _receiver.Url));

        _client = _api.CreateClient();
    }

    [When("an {string} event is submitted")]
    public async Task WhenAnEventIsSubmitted(string eventType)
    {
        _eventId = Guid.NewGuid().ToString();

        _submission = await _client!.PostAsJsonAsync(
            "/v1/events",
            new { id = _eventId, type = eventType, data = new { invoiceId = "INV-100" } });
    }

    [Then("the event is accepted")]
    public void ThenTheEventIsAccepted()
    {
        Assert.That(_submission!.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
    }

    [Then("the receiver eventually gets the event")]
    public async Task ThenTheReceiverEventuallyGetsTheEvent()
    {
        var timer = Stopwatch.StartNew();

        while (timer.Elapsed < DeliveryTimeout)
        {
            if (_receiver!.Deliveries.Any(delivery => delivery.Contains(_eventId!)))
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(50));
        }

        Assert.Fail($"Event {_eventId} was not delivered within {DeliveryTimeout.TotalSeconds} seconds.");
    }

    public async ValueTask DisposeAsync()
    {
        _submission?.Dispose();
        _client?.Dispose();

        if (_api is not null)
        {
            await _api.DisposeAsync();
        }

        if (_receiver is not null)
        {
            await _receiver.DisposeAsync();
        }
    }
}
