namespace AsPirate.ShipTransmitter.Interfaces;

public interface ITransmitter
{
    Task BringOnline(int shipId);
    Task TakeOffline(int shipId);
    Task TransmitLocation(int shipId);
}
