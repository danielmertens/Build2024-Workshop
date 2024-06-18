using AsPirate.ShipTransmitter;
using AsPirate.ShipTransmitter.ApiClients;
using AsPirate.ShipTransmitter.Interfaces;
using AsPirate.ShipTransmitter.Models;
using AsPirate.ShipTransmitter.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ITransmitter, FakeTransmitter>();

builder.Services.AddHttpClient<ShipManagerApiClient>(static client => client.BaseAddress = new("https+http://shipmanagerapi"));
builder.Services.AddHttpClient<PathFinderApiClient>(static client => client.BaseAddress = new("https+http://pathfinderapi"));

builder.Services.AddSingleton((provider) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new ShipContext
    {
        ShipId = configuration.GetValue<int>("ShipId")
    };
});

var host = builder.Build();
host.Run();
