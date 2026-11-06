using Redeliver.Api.Features.DeliverEvents;
using Redeliver.Api.Features.SubmitEvent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EventQueue>();
builder.Services.AddSingleton(CreateReceiverOptions);
builder.Services.AddHttpClient<EventSender>();
builder.Services.AddHostedService<DeliveryWorker>();

var app = builder.Build();

app.MapSubmitEvent();

app.Run();

static ReceiverOptions CreateReceiverOptions(IServiceProvider services)
{
    var configuration = services.GetRequiredService<IConfiguration>();
    var receiverUrl = configuration.GetValue<string>("Receiver:Url")!;

    return new ReceiverOptions(new Uri(receiverUrl));
}

public partial class Program;
