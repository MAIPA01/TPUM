namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void ClientConnectedEventHandler(object? source);

    public interface INotifyClientConnected
    {
        event ClientConnectedEventHandler? ClientConnected;
    }
}