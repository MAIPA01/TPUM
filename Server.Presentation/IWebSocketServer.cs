using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    internal interface IWebSocketServer : INotifyOnClientRequestReceived, INotifyClientDisconnected
    {
        Task StartAsync();

        Task SendAsync(Guid clientId, string clientResponse);

        Task SendToClientsAsync(List<Guid> clientsIds, string broadcastResponse);

        Task BroadcastAsync(string broadcastResponse);
    }
}
