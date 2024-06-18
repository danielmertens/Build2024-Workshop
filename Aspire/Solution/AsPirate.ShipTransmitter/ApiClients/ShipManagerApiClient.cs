using System.Net.Http.Json;

namespace AsPirate.ShipTransmitter.ApiClients;

public class ShipManagerApiClient
{
    private readonly HttpClient _client;

    public ShipManagerApiClient(HttpClient httpClient)
    {
        _client = httpClient;
    }

    public async Task BringOnline(int shipId)
    {
        await _client.PostAsJsonAsync($"/ships/{shipId}/online", new { });
    }

    public async Task TakeOffline(int shipId)
    {
        await _client.PostAsJsonAsync($"/ships/{shipId}/offline", new { });
    }
}
