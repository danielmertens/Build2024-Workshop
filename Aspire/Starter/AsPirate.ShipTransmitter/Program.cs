using AsPirate.ShipTransmitter;
using AsPirate.ShipTransmitter.ApiClients;
using AsPirate.ShipTransmitter.Interfaces;
using AsPirate.ShipTransmitter.Models;
using AsPirate.ShipTransmitter.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ITransmitter, FakeTransmitter>();

builder.Services.AddSingleton((provider) =>
{
    return new ShipContext
    {
        ShipId = 0
    };
});

var host = builder.Build();
host.Run();
