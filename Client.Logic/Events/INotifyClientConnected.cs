namespace TPUM.Client.Logic.Events
{
    public delegate void ClientConnectedEventHandler(object? source);

    public interface INotifyClientConnected
    {
        event ClientConnectedEventHandler? ClientConnected;
    }
}