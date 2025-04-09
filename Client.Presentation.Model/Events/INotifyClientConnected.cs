namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void ClientConnectedEventHandler(object? source, ClientConnectedEventArgs e);
    public class ClientConnectedEventArgs : EventArgs
    {
        public static new readonly ClientConnectedEventArgs Empty = new();
        public ClientConnectedEventArgs()
        {
        }
    }

    public interface INotifyClientConnected
    {
        event ClientConnectedEventHandler? ClientConnected;
    }
}