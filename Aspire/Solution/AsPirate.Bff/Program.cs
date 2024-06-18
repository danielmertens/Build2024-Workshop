using AsPirate.Bff.ApiClients;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddHttpClient<ShipManagerApi>(static client => client.BaseAddress = new("https+http://shipmanagerapi"));
builder.Services.AddHttpClient<PathFinderApiClient>(static client => client.BaseAddress = new("https+http://pathfinderapi"));

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors(static builder =>
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());

app.MapGet("/boats/summary", async (ShipManagerApi ledgerApi, PathFinderApiClient pathFinderApi) =>
{
    var ships = await ledgerApi.GetActiveShipsSummary();
    var locations = await pathFinderApi.FindLocationForShips(ships.Select(s => s.Id).ToArray());

    return Map(ships, locations);
});

app.MapGet("/boats/{id:int}", async (ShipManagerApi ledgerApi, int id) =>
{
    return await ledgerApi.GetShip(id);
});

SummaryResponse Map(ShipSummary[] ledgerShips, ShipLocation[] locations)
{
    return new SummaryResponse(ledgerShips.Select(s =>
    {
        var location = locations.FirstOrDefault(l => l.Id == s.Id);
        return new ShipDto(s.Id, s.Name, location?.Coordinate.X ?? 0, location?.Coordinate.Y ?? 0, location?.Line ?? [], location?.Heading.X ?? 0);
    }).ToArray());
}

app.Run();

record SummaryResponse(ShipDto[] Ships)
{
    public int ShipCount => Ships.Length;
}

record ShipDto(int Id, string Name, int X, int Y, Coordinate[] Line, float Heading);
