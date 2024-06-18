# Aspire Workshop

In this workshop we'll be working with the AsPirate Tracking Map application. Below is the architecture of our project with some explanation of each service. During this exercise we will be adding Aspire to the project and making the application work.

![Exercise architecture](./Images/Architecture.drawio.png)

Frontend: This is a react frontend. The frontend will display a map with the location of each active ship.
BFF: A Backend For Frontend web api that is provides public endpoints for the frontend to retrieve information.
Ships Manager: This web api maintains all information of ships.
PathFinder: This is the navigation application. For now this web application only stores the location of each ship.
Transmitter: The transmitter is a IOT service that runs on each ship. Each transmitter should transmit data about the ship it's on to the Ship manager and PathFinder services.

## Step 1 - Project exploration

Navigate to the "Starter" folder, open the solution and look at the project structure. You should be able to spot all the services mentioned above. There will also be an additional project named "AsPirate.AppHost". This is our Aspire Host file that we will be using to Aspire-ify our project.

The AppHost project contains only one "program.cs" file that is almost empty. 

```c#
var builder = DistributedApplication.CreateBuilder(args);

builder.Build().Run();
```

You'll notice a `DistributedApplication` builder in the AppHost program.cs file. This may seem familiar since it's similar to the ASP.NET program.cs.

Now run the solution by setting the AppHost project as the startup project of your solution or run `dotnet run` from the apphost folder.

Our aspire application will be started and a browser will open with an empty dashboard.

![Empty dashboard](./Images/empty-dashboard.jpg)

## Step 2 - Let's define our infrastructure

To run other projects in the solution, we need to define to our AppHost what to run and how to run it. Let's start by adding our BFF first.

We need to add the Bff project to our builder. This is very straightforward.

```c#
builder.AddProject<Projects.AsPirate_Bff>("bff");
```

Note that the static class `Projects.AsPirate_Bff` is a class that was source generated by Aspire. A reference like this is created for each dependency added to the AppHost project. The name that we specify as parameter of the method is the name used in the dashboard for the service. It is advised to keep the name one word without spaces because it will have more uses for other features later on in the exercise.

If we run our project now, we will see that the Bff project appears in the dashboard and has a link to its swagger page.

![Dashboard with bff](./Images/dashboard-with-bff.jpg)

Now lets define our ShipsManager service and PathFinder service as well:

```c#
builder.AddProject<Projects.AsPirate_ShipsManager>("shipmanagerapi");
builder.AddProject<Projects.AsPirate_PathFinderApi>("pathfinderapi");
```

To define our react we need to take another approach. The `AddProject` method can only be used for other C# projects in the same solution. Fear not, Aspire offers a wide array for integration with a lot of different thing. By adding the `Aspire.Hosting.NodeJs` nuget package (Already added!) we can integrate with any type of node application. Simply add the following code to the builder

```c#
builder.AddNpmApp("react", "../AsPirate.React")
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT"); // We will be forwarding a random port on which the frontend will run.
```

This code will start the react application by calling `npm start` in the specified folder when the AppHost is starting up.

If you run the application again your dashboard should contain the frontend, bff, ships manager and pathfinder services. Make sure to check that the frontend works and check the Console, Structured, Traces and Metrics tab at the left on the dashboard.

![Dashboard with all services](./Images/dashboard-with-all-services.jpg)

Once you're done exploring you should have noticed that the frontend is working and you are able to see the console output of each application you started. Unfortunately the structured logging and metrics don't seem to be working yet. We will be fixing this in the next step.

## Step 3 - Setting service defaults

To add structured logging, we need to apply open telemetry standard logging to all our services. This can be a lot of work to do right. Luckily Aspire can take care of this for us. The Aspire has even defined a very handy base template to get started with.

Right-click the solution and add a new project to our solution. In the project templates explorer search for the `aspire service defaults` template.

![Service default template selection](./Images/service-defaults-template-selection.jpg)

Select this template and give it the name `AsPirate.ServiceDefaults`. It's important to make project name end with 'ServiceDefaults'. This will enable Visual Studio to add an extra option to when creating any other project in the future named "Add Service Default". This will make sure the service defaults is already included in the new project AND the new project will automatically be added to the AppHost file because it recognizes that we are working in an Aspire project.

Now that we have service defaults you can have a look at the project. You'll notice that it only contains an Extensions.cs file that exposes two important public extension methods `AddServiceDefaults` and `MapDefaultEndpoints`.

The `AddServiceDefaults` method will add services like open telemetry, health checks, service discovery and standard resiliency to our projects. Those are already fully configured by the standards of the Aspire team.

The `MapDefaultEndpoints` adds some default health endpoints to our project for development.

The purpose of this project is to add default configuration that is needed in every project in the solution. Any configuration that you want to add that is needed by every project should be done in these methods.

To continue with the exercise, add these two methods to the program.cs file of the bff, ships manager and pathfinder projects.

```c#
// other code ommited
builder.AddServiceDefaults();
// ...
app.MapDefaultEndpoints();
```

Now when we start our project we will notice in our dashboard that open telemetry is integrated and we will be able to see traces, structured logs and metrics. Explore the dashboard a bit.

![Dashboard structured logs](./Images/dashboard-structured-logs.jpg)
![Dashboard traces](./Images/dashboard-traces.jpg)

## Step 4 - Service discovery

In the service defaults a "ServiceDiscovery" module is added to your services. This is a very handy nuget package extension that helps with making calls to other services. Currently when the BFF talks to the manager and pathfinder services, it does so with a hard-coded url and port. This is not ideal.

```c#
// Currently in AsPirate.Bff program.cs
builder.Services.AddHttpClient<ShipManagerApi>(static client => client.BaseAddress = new("https://localhost:7122"));
builder.Services.AddHttpClient<PathFinderApiClient>(static client => client.BaseAddress = new("https://localhost:7176"));
```

With Service Discovery we can make this a lot easier. To start we need to tell Aspire and Service Discovery that the Bff needs a reference to the manager and pathfinder service. This can be configured in the AppHost as follows:

```c#
var shipManagerApi = builder.AddProject<Projects.AsPirate_ShipsManager>("shipmanagerapi");
var pathfinderApi = builder.AddProject<Projects.AsPirate_PathFinderApi>("pathfinderapi");

var bff = builder.AddProject<Projects.AsPirate_Bff>("bff")
    .WithReference(shipManagerApi)
    .WithReference(pathfinderApi);
```

We simply tell the builder there is a reference to some other projects that are already added. What this will do in the code behind is add some environment variable specifically to the BFF service. You can see this by viewing the details of the bff service in the dashboard.

![Service Discovery environment variables](./Images/service-discovery-bff-details.jpg)

Notice that the name we gave to each project in our AppHost project is used as part of the environment variable name.

Now we just need to let our Discovery Service package know where to use these values. We can do this by simply changing the urls we use to register our HttpClient with the name of our services as specified in the AppHost project. So the code becomes:

```c#
builder.Services.AddHttpClient<ShipManagerApi>(static client => client.BaseAddress = new("https://shipmanagerapi"));
builder.Services.AddHttpClient<PathFinderApiClient>(static client => client.BaseAddress = new("https://pathfinderapi"));
```

The Service Discovery will replace the `pathfinderapi` and `shipmanagerapi` name it find in the url with the url it finds in the environment variables. This is not only handy to improve local development but is also useful when deploying to different environments. The service discovery package is also able to hook into the kubernetes service discovery feature.

## Step 5 - Time to add some ships

Now to start adding our transmitter into the mix we need to finish the configuration of our transmitter. Open the `program.cs` of the ShipTransmitter project.

Start by adding a reference to the ServiceDefaults project and include the service defaults. We don't need to map any default endpoints here since this is not an api project.

```c#
builder.AddServiceDefaults();
```

Next we can add the HttpClients to the service using Service Discovery.

```c#
builder.Services.AddHttpClient<ShipManagerApiClient>(static client => client.BaseAddress = new("https+http://shipmanagerapi"));
builder.Services.AddHttpClient<PathFinderApiClient>(static client => client.BaseAddress = new("https+http://pathfinderapi"));
```

The last thing we need to setup in this service is specifying the ShipId. Any value from 1 to 10 would work here since those are the id's known by our manager. We would however like to have some control over which ship we are transmitting from, from our AppHost. Therefor we are going to read the ShipId from our environment variables as follows:

```c#
builder.Services.AddSingleton((provider) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new ShipContext
    {
        ShipId = configuration.GetValue<int>("ShipId")
    };
});
```

This means that we will need to specify this environment variable somewhere in our AppHost. Switch to the AppHost to add the transmitter to your infrastructure.

To add a single transmitter to the project we can add it as a new project in our AppHost and make the correct references like this.

```c#
builder.AddProject<Projects.AsPirate_ShipTransmitter>($"aspirate-shiptransmitter")
    .WithReference(shipManagerApi)
    .WithReference(pathfinderApi)
    .WithEnvironment("ShipId", "1");
```

This adds the transmitter with a reference to the manager and pathfinder and sets the `ShipId` environment variable to 1.

For development it is however handy if we can test with multiple transmitters active. In a traditional project this would be a problem, starting the same project multiple times in debug mode. With Aspire however, this becomes a piece of cake.

We can simply create a loop in our AppHost and create multiple transmitters:

```c#
for (int i = 1; i < 6; i++)
{
    builder.AddProject<Projects.AsPirate_ShipTransmitter>($"aspirate-shiptransmitter-{i}")
        .WithReference(shipManagerApi)
        .WithReference(pathfinderApi)
        .WithEnvironment("ShipId", i.ToString());
}
```
## Step 6 - Getting to the cloud

With Aspire it's easy to deploy your application to the cloud.

First make sure you have the [Azure developer cli](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/install-azd) installed and are logged in.

Now `cd` to the AppHost directory in a terminal and execute the command `azd init`. This command will analyze your project and recognize that it is an Aspire application. Follow the prompts and specify an environment name.

Once this has run successfully you can deploy your app with `azd up`. Select the subscription you'd like to use and the region you'd like to deploy in. This may take some time.

Once this process is finished you can find your running application in container apps on azure together with a Aspire Dashboard with working open telemetry logging.

**Warning: Don't forget to delete your resources after your done.**