namespace TPUM.Client.Data.Events
{
    internal delegate void MessageReceivedEventHandler(object? source, string message);

    internal interface INotifyMessageReceived
    {
        event MessageReceivedEventHandler? MessageReceived;
    }
}