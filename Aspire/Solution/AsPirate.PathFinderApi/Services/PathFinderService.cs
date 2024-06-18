namespace AsPirate.PathFinderApi.Services;

public interface IPathFinderService
{
    void AddShipCoordinate(int shipId, Coordinate coordinate, Heading heading);
    ShipLocation[] GetShipLocations(int[] ids);
}

public class PathFinderService : IPathFinderService
{
    private readonly static Dictionary<int, ShipLocation> Ships = [];

    public ShipLocation[] GetShipLocations(int[] ids)
    {
        var locations = new List<ShipLocation>();

        foreach (var id in ids)
        {
            if (!Ships.ContainsKey(id))
            {
                locations.Add(new ShipLocation
                {
                    Id = id,
                    Coordinate = new() { X = 25, Y = 25 },
                    Heading = new() { X = 0, Y = 0 },
                    Line = []
                });
            }
            else
            {
                var ship = Ships[id];
                locations.Add(new ShipLocation
                {
                    Id = ship.Id,
                    Coordinate = ship.Coordinate,
                    Heading = ship.Heading,
                    Line = ship.Line.TakeLast(10).ToArray()
                });
            }
        }

        return locations.ToArray();
    }

    public void AddShipCoordinate(int shipId, Coordinate coordinate, Heading heading)
    {
        ShipLocation location;
        if (Ships.ContainsKey(shipId))
        {
            location = Ships[shipId];
            location.Line = [.. location.Line, location.Coordinate];
            location.Coordinate = coordinate;
            location.Heading = heading;
        }
        else
        {
            Ships.Add(shipId, new ShipLocation
            {
                Id = shipId,
                Coordinate = coordinate,
                Heading = heading,
                Line = []
            });
        }
    }
}

public class ShipLocation
{
    public int Id { get; set; }
    public required Coordinate Coordinate { get; set; }
    public Coordinate[] Line { get; set; } = [];
    public Heading Heading { get; set; }
}

public class Coordinate
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class Heading
{
    public required float X { get; set; }
    public required float Y { get; set; }
}