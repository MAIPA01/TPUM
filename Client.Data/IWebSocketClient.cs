using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    internal interface IWebSocketClient : INotifyMessageReceived, INotifyClientConnected, IDisposable
    {
        Task ConnectAsync(string uri);

        Task SendAsync(string xml);

        Task DisconnectAsync();
    }
}
