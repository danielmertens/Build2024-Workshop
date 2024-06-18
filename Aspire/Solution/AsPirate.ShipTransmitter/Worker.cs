using AsPirate.ShipTransmitter.Interfaces;
using AsPirate.ShipTransmitter.Models;

namespace AsPirate.ShipTransmitter;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ITransmitter _transmitter;
    private readonly ShipContext _shipContext;

    public Worker(ILogger<Worker> logger, ITransmitter transmitter, ShipContext shipContext)
    {
        _logger = logger;
        _transmitter = transmitter;
        _shipContext = shipContext;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        await _transmitter.BringOnline(_shipContext.ShipId);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("{time} - Transmitting ship location.", DateTimeOffset.Now);
                await _transmitter.TransmitLocation(_shipContext.ShipId);
            }
            await Task.Delay(5000, stoppingToken);
        }
    }
}
