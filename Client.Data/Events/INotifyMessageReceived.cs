namespace TPUM.Client.Data.Events
{
    public delegate void MessageReceivedEventHandler(object? source, MessageReceivedEventArgs e);
    public class MessageReceivedEventArgs : EventArgs
    {
        public string XmlMessage { get; }

        public MessageReceivedEventArgs(string xml)
        {
            XmlMessage = xml;
        }
    }

    public interface INotifyMessageReceived
    {
        event MessageReceivedEventHandler? MessageReceived;
    }
}