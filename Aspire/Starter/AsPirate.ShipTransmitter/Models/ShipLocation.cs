namespace AsPirate.ShipTransmitter.Models;

public class ShipLocation
{
    public required Coordinate Coordinate { get; set; }
    public required Heading Heading { get; set; }
}

public class Coordinate
{
    public required int X { get; set; }
    public required int Y { get; set; }
}

public class Heading
{
    public required float X { get; set; }
    public required float Y { get; set; }
}

public class ShipContext
{
    public int ShipId { get; set; }
}