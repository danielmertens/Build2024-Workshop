namespace AsPirate.ShipsManager;

public static class ShipCache
{
    public static Ship[] Ships { get; } = [
        new Ship
        {
            Id = 1,
            Name = "Black Pearl",
            Type = "Galleon",
            BuildYear = 1660,
            Dimensions = new Dimensions { Length = 165, Beam = 40, Draft = 15 },
            Armament = new Armament { Cannons = 32 },
            Speed = 25,
            Maneuverability = 90,
            Ownership = [new Ownership { Captain = "Jack Sparrow" }]
        },
        new Ship
        {
            Id = 2,
            Name = "Queen Anne's Revenge",
            Type = "Frigate",
            BuildYear = 1710,
            Dimensions = new Dimensions { Length = 150, Beam = 35, Draft = 13 },
            Armament = new Armament { Cannons = 40 },
            Speed = 20,
            Maneuverability = 80,
            Ownership = [new Ownership { Captain = "Blackbeard" }]
        },
        new Ship
        {
            Id = 3,
            Name = "Flying Dutchman",
            Type = "Ghost Ship",
            BuildYear = 1600,
            Dimensions = new Dimensions { Length = 170, Beam = 38, Draft = 14 },
            Armament = new Armament { Cannons = 50 },
            Speed = 30,
            Maneuverability = 100,
            Ownership = [new Ownership { Captain = "Davy Jones" }]
        },
        new Ship
        {
            Id = 4,
            Name = "Jolly Roger",
            Type = "Brigantine",
            BuildYear = 1690,
            Dimensions = new Dimensions { Length = 120, Beam = 30, Draft = 12 },
            Armament = new Armament { Cannons = 20 },
            Speed = 18,
            Maneuverability = 85,
            Ownership = [new Ownership { Captain = "Captain Hook" }]
        },
        new Ship
        {
            Id = 5,
            Name = "Adventure Galley",
            Type = "Hybrid",
            BuildYear = 1695,
            Dimensions = new Dimensions { Length = 124, Beam = 34, Draft = 14 },
            Armament = new Armament { Cannons = 34 },
            Speed = 16,
            Maneuverability = 75,
            Ownership = [new Ownership { Captain = "William Kidd" }]
        },
        new Ship
        {
            Id = 6,
            Name = "Whydah",
            Type = "Galleon",
            BuildYear = 1715,
            Dimensions = new Dimensions { Length = 110, Beam = 28, Draft = 11 },
            Armament = new Armament { Cannons = 28 },
            Speed = 15,
            Maneuverability = 70,
            Ownership = [new Ownership { Captain = "Black Sam Bellamy" }]
        },
        new Ship
        {
            Id = 7,
            Name = "Royal Fortune",
            Type = "Frigate",
            BuildYear = 1696,
            Dimensions = new Dimensions { Length = 140, Beam = 32, Draft = 13 },
            Armament = new Armament { Cannons = 26 },
            Speed = 22,
            Maneuverability = 80,
            Ownership = [new Ownership { Captain = "Bartholomew Roberts" }]
        },
        new Ship
        {
            Id = 8,
            Name = "Golden Hind",
            Type = "Galleon",
            BuildYear = 1577,
            Dimensions = new Dimensions { Length = 120, Beam = 28, Draft = 11 },
            Armament = new Armament { Cannons = 22 },
            Speed = 17,
            Maneuverability = 75,
            Ownership = [new Ownership { Captain = "Francis Drake" }]
        },
        new Ship
        {
            Id = 9,
            Name = "Sea Shadow",
            Type = "Stealth Ship",
            BuildYear = 1985,
            Dimensions = new Dimensions { Length = 164, Beam = 68, Draft = 15 },
            Armament = new Armament { Cannons = 0 },
            Speed = 20,
            Maneuverability = 90,
            Ownership = [new Ownership { Captain = "Invisible Man" }]
        },
        new Ship
        {
            Id = 10,
            Name = "HMS Interceptor",
            Type = "Sloop",
            BuildYear = 1700,
            Dimensions = new Dimensions { Length = 90, Beam = 24, Draft = 10 },
            Armament = new Armament { Cannons = 14 },
            Speed = 25,
            Maneuverability = 95,
            Ownership = [new Ownership { Captain = "James Norrington" }]
        }
    ];
}

public class Ship
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int BuildYear { get; set; }
    public required Dimensions Dimensions { get; set; }
    public Armament? Armament { get; set; }
    public int Speed { get; set; }
    public int Maneuverability { get; set; }

    public required Ownership[] Ownership { get; set; }

    public bool Online { get; set; }
}

public class Ownership
{
    public required string Captain { get; set; }
}

public class Armament
{
    public int Cannons { get; set; }
}

public class Dimensions
{
    public int Length { get; set; }
    public int Beam { get; set; }
    public int Draft { get; set; }
}
