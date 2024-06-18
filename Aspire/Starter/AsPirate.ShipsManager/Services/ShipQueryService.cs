
namespace AsPirate.ShipsManager.Services;

public interface IShipQueryService
{
    ShipSummary[] GetAllActiveShipsSummary();
    Ship? GetShip(int id);
}

public class ShipQueryService : IShipQueryService
{
    public ShipSummary[] GetAllActiveShipsSummary()
    {
        return ShipCache.Ships
            .Where(s => s.Online)
            .Select(Summarize)
            .ToArray();
    }

    private ShipSummary Summarize(Ship ship) =>
        new ShipSummary(ship.Id, ship.Name, ship.Type, ship.Ownership.Last().Captain);

    public Ship? GetShip(int id) => ShipCache.Ships.FirstOrDefault(s => s.Id == id);
}

public record ShipSummary(int Id, string Name, string Type, string Captain);
