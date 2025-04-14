namespace TPUM.Client.Presentation.ViewModel.Events
{
    public delegate void ClientConnectedEventHandler(object? source);

    public interface INotifyClientConnected
    {
        event ClientConnectedEventHandler? ClientConnected;
    }
}