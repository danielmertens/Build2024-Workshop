using AsPirate.ShipsManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IShipQueryService, ShipQueryService>();
builder.Services.AddScoped<IShipCommandService, ShipCommandService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/ships", (IShipQueryService ledgerQueryService) =>
{
    return ledgerQueryService.GetAllActiveShipsSummary();
});

app.MapGet("/ships/{id:int}", (int id, IShipQueryService ledgerQueryService) =>
{
    return ledgerQueryService.GetShip(id);
});

app.MapPost("/ships/{id:int}/online", (int id, IShipCommandService ledgerCommandService) =>
{
    ledgerCommandService.SetShipStatus(id, true);
});

app.MapPost("/ships/{id:int}/offline", (int id, IShipCommandService ledgerCommandService) =>
{
    ledgerCommandService.SetShipStatus(id, false);
});

app.Run();
