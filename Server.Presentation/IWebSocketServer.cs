using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    internal interface IWebSocketServer : INotifyOnClientRequestReceived
    {
        Task StartAsync();

        Task SendAsync(Guid clientId, string clientResponse);

        Task BroadcastAsync(string broadcastResponse);
    }
}
