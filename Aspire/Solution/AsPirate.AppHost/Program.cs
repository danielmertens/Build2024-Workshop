var builder = DistributedApplication.CreateBuilder(args);

var shipManagerApi = builder.AddProject<Projects.AsPirate_ShipsManager>("shipmanagerapi");
var pathfinderApi = builder.AddProject<Projects.AsPirate_PathFinderApi>("pathfinderapi");

var bff = builder.AddProject<Projects.AsPirate_Bff>("bff")
    .WithExternalHttpEndpoints()
    .WithReference(shipManagerApi)
    .WithReference(pathfinderApi);

for (int i = 1; i < 6; i++)
{
    builder.AddProject<Projects.AsPirate_ShipTransmitter>($"aspirate-shiptransmitter-{i}")
        .WithReference(shipManagerApi)
        .WithReference(pathfinderApi)
        .WithEnvironment("ShipId", i.ToString());
}


builder.AddNpmApp("react", "../AsPirate.React")
    .WithReference(bff)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
