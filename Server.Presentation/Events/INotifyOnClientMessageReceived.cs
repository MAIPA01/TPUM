namespace TPUM.Server.Presentation.Events
{
    public delegate void ClientMessageReceivedEventHandler(object? source, Guid clientId, string message);

    public interface INotifyOnClientMessageReceived
    {
        event ClientMessageReceivedEventHandler? ClientMessageReceived;
    }
}
