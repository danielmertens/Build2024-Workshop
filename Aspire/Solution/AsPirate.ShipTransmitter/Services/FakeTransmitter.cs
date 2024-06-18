using AsPirate.ShipTransmitter.ApiClients;
using AsPirate.ShipTransmitter.Interfaces;
using AsPirate.ShipTransmitter.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;

namespace AsPirate.ShipTransmitter.Services;

public class FakeTransmitter : ITransmitter
{
    private Image<Rgba32>? _mapMask = null;
    private readonly LinkedList<ShipLocation> _memory = [];
    private readonly ShipManagerApiClient _shipManagerApiClient;
    private readonly PathFinderApiClient _pathFinderApiClient;
    private bool _transmitterInitialized = false;

    public FakeTransmitter(ShipManagerApiClient shipManagerApiClient,
        PathFinderApiClient pathFinderApiClient)
    {
        _shipManagerApiClient = shipManagerApiClient;
        _pathFinderApiClient = pathFinderApiClient;
    }

    public async Task BringOnline(int shipId)
    {
        await Initialize();
        await _shipManagerApiClient.BringOnline(shipId);
    }

    public async Task TakeOffline(int shipId)
    {
        await _shipManagerApiClient.TakeOffline(shipId);
    }

    public async Task TransmitLocation(int shipId)
    {
        if (!_transmitterInitialized) return;

        ShipLocation newLocation;
        if (!_memory.Any())
        {
            newLocation = CreateFirstLocation();
        }
        else
        {
            newLocation = GenerateNextLocation(_memory.Last());
        }

        _memory.AddLast(newLocation);
        await _pathFinderApiClient.TransmitLocation(shipId, newLocation);
    }

    private ShipLocation GenerateNextLocation(ShipLocation shipLocation)
    {
        var vect = new Vector2(shipLocation.Heading.X, shipLocation.Heading.Y);
        while (true)
        {
            // Rotate heading slightly randomly. (Perlin noise)
            vect = Vector2.Transform(vect,
                Matrix3x2.CreateRotation((float)(Random.Shared.NextDouble() * 2 - 1)));

            var newX = (int)(shipLocation.Coordinate.X + vect.X * 40);
            var newY = (int)(shipLocation.Coordinate.Y + vect.Y * 40);

            if (newX > 0 && newY > 0
                && newX < _mapMask.Width && newY < _mapMask.Height
                && _mapMask[newX, newY] == Rgba32.ParseHex("000000"))
            {
                return new ShipLocation
                {
                    Coordinate = new() { X = newX, Y = newY },
                    Heading = new() { X = vect.X, Y = vect.Y }
                };
            }
        }
    }

    private ShipLocation CreateFirstLocation()
    {
        while (true)
        {
            var x = Random.Shared.Next(50, _mapMask.Width - 100);
            var y = Random.Shared.Next(50, _mapMask.Height - 100);

            var vect = new Vector2(
                (float)(Random.Shared.NextDouble() * 2 - 1),
                (float)(Random.Shared.NextDouble() * 2 - 1));
            vect = Vector2.Normalize(vect);

            var color = _mapMask[x, y];
            if (color == Rgba32.ParseHex("000000"))
            {
                return new ShipLocation()
                {
                    Coordinate = new Coordinate { X = x, Y = y },
                    Heading = new Heading
                    {
                        X = vect.X,
                        Y = vect.Y
                    }
                };
            }
        }
    }

    private async Task Initialize()
    {
        _mapMask = await Image.LoadAsync<Rgba32>("./Assets/sea_map_mask.jpg");

        _transmitterInitialized = true;
    }
}
