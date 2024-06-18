namespace AsPirate.ShipsManager.Services;

public interface IShipCommandService
{
    void SetShipStatus(int shipId, bool online);
}

public class ShipCommandService : IShipCommandService
{
    private readonly ILogger<ShipCommandService> _logger;

    public ShipCommandService(ILogger<ShipCommandService> logger)
    {
        _logger = logger;
    }

    public void SetShipStatus(int shipId, bool online)
    {
        var ship = ShipCache.Ships.FirstOrDefault(s => s.Id == shipId);

        if (ship != null)
        {
            ship.Online = online;
        }
        else
        {
            _logger.LogWarning("Attempting to bring online ship {shipId} but ship was not found in ledger.", new { shipId });
        }
    }
}
