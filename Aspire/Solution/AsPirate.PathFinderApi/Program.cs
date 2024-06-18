using AsPirate.PathFinderApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPathFinderService, PathFinderService>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost("/ships/find", (int[] shipIds, IPathFinderService pathFinderService) =>
{
    return pathFinderService.GetShipLocations(shipIds);
});

app.MapPost("/ships/register-location", (LocationDto dto, IPathFinderService pathFinderService) =>
{
    pathFinderService.AddShipCoordinate(dto.shipId, dto.Coordinate, dto.Heading);
});

app.Run();

public class LocationDto
{
    public int shipId { get; set; }
    public Coordinate Coordinate { get; set; }
    public Heading Heading { get; set; }
}