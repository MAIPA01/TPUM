using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    internal interface IWebSocketClient : IDisposable, INotifyMessageReceived, INotifyClientConnected
    {
        Task ConnectAsync(string uri);

        Task SendAsync(string xml);

        Task<string> ReceiveAsync();

        Task<string> SendAndReceiveAsync(string xml);

        Task DisconnectAsync();
    }
}
