namespace TPUM.Server.Presentation.Events
{
    internal delegate void ClientMessageReceivedEventHandler(object? source, Guid clientId, string message);

    internal interface INotifyOnClientMessageReceived
    {
        event ClientMessageReceivedEventHandler? ClientMessageReceived;
    }
}
