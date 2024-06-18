using AsPirate.ShipTransmitter.Models;
using System.Net.Http.Json;

namespace AsPirate.ShipTransmitter.ApiClients;

public class PathFinderApiClient
{
    private readonly HttpClient _client;

    public PathFinderApiClient(HttpClient httpClient)
    {
        _client = httpClient;
    }

    public async Task TransmitLocation(int shipId, ShipLocation location)
    {
        await _client.PostAsJsonAsync("/ships/register-location", new
        {
            ShipId = shipId,
            location.Coordinate,
            location.Heading
        });
    }
}
