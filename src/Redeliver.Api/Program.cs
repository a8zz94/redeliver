using Redeliver.Api.Features.DeliverEvents;
using Redeliver.Api.Features.SubmitEvent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EventQueue>();
builder.Services.AddSingleton(services =>
    new ReceiverOptions(new Uri(services.GetRequiredService<IConfiguration>().GetValue<string>("Receiver:Url")!)));
builder.Services.AddHttpClient<EventSender>();
builder.Services.AddHostedService<DeliveryWorker>();

var app = builder.Build();

app.MapSubmitEvent();

app.Run();

public partial class Program;
