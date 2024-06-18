namespace AsPirate.Bff.ApiClients;

public class ShipManagerApi(HttpClient httpClient)
{
    public async Task<ShipSummary[]> GetActiveShipsSummary()
    {
        return await httpClient.GetFromJsonAsync<ShipSummary[]>("/ships") ?? [];
    }

    public async Task<Ship?> GetShip(int shipId)
    {
        return await httpClient.GetFromJsonAsync<Ship>($"/ships/{shipId}");
    }
}

public record ShipSummary(int Id, string Name, string Type, string Captain);

public record Ship
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Type { get; init; }
    public int BuildYear { get; init; }
    public required Dimensions Dimensions { get; init; }
    public Armament? Armament { get; init; }
    public int Speed { get; init; }
    public int Maneuverability { get; init; }

    public required Ownership[] Ownership { get; init; }
}

public record Ownership
{
    public required string Captain { get; init; }
}

public record Armament
{
    public int Cannons { get; init; }
}

public record Dimensions
{
    public int Length { get; init; }
    public int Beam { get; init; }
    public int Draft { get; init; }
}
