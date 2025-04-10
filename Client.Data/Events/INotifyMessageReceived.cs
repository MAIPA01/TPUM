namespace TPUM.Client.Data.Events
{
    public delegate void MessageReceivedEventHandler(object? source, string message);

    public interface INotifyMessageReceived
    {
        event MessageReceivedEventHandler? MessageReceived;
    }
}