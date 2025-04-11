using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    internal interface IWebSocketClient : INotifyResponseReceived, INotifyBroadcastReceived, INotifyClientConnected, IDisposable
    {
        Task ConnectAsync(string uri);

        Task SendAsync(string xml);

        Task DisconnectAsync();
    }
}
