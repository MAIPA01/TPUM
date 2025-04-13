namespace TPUM.Server.Presentation.Events
{
    public delegate void ClientDisconnectedEventHandler(object? source, Guid clientId);
    public interface INotifyClientDisconnected
    {
        event ClientDisconnectedEventHandler? ClientDisconnected;
    }
}
