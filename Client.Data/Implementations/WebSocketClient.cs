using System.Net.WebSockets;
using System.Text;
using TPUM.Client.Data.Events;
using TPUM.XmlShared;
using TPUM.XmlShared.Response;
using TPUM.XmlShared.Response.Broadcast;
using TPUM.XmlShared.Response.Client;
using TPUM.XmlShared.Response.Subscribe;

namespace TPUM.Client.Data
{
    internal class WebSocketClient : IWebSocketClient
    {
        private readonly ClientWebSocket _client = new();
        private readonly CancellationTokenSource _cts = new();
        private const int ReconnectIntervalInSeconds = 5;

        public event ClientConnectedEventHandler? ClientConnected;
        public event ResponseReceivedEventHandler? ResponseReceived;
        public event SubscribeReceivedEventHandler? SubscribeReceived;
        public event BroadcastReceivedEventHandler? BroadcastReceived;

        public async Task ConnectAsync(string uri)
        {
            while (_client.State != WebSocketState.Open)
            {
                try
                {
                    await _client.ConnectAsync(new Uri(uri), CancellationToken.None);
                }
                catch (Exception ex)
                {
                    await Task.Delay(ReconnectIntervalInSeconds * 1000);
                }
            }

            ClientConnected?.Invoke(this);
            _ = Task.Run(ReceiveLoop);
        }

        public async Task SendAsync(string request)
        {
            var bytes = Encoding.UTF8.GetBytes(request);
            await _client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        private async Task ReceiveLoop()
        {
            var buffer = new byte[4096];
            while (!_cts.IsCancellationRequested)
            {
                var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                if (result.MessageType != WebSocketMessageType.Binary) continue;
                var xml = Encoding.UTF8.GetString(buffer, 0, result.Count);
                ProcessMessage(xml);
            }
        }

        private void ProcessMessage(string message)
        {
            if (!XmlSerializerHelper.TryDeserialize<Response>(message, out var response)) return;

            switch (response.ContentType)
            {
                case ResponseType.Client:
                    OnResponseReceived((ClientResponseContent)response.Content);
                    break;
                case ResponseType.Broadcast:
                    OnBroadcastReceived((BroadcastResponseContent)response.Content);
                    break;
                case ResponseType.Subscribe:
                    OnSubscribeReceived((SubscribeResponseContent)response.Content);
                    break;
                default:
                    return;
            }
        }

        private static async Task DisconnectSocketAsync(ClientWebSocket client)
        {
            if (client.State == WebSocketState.Open || client.State == WebSocketState.CloseReceived)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting",
                    CancellationToken.None);
            }
            else if (client.State == WebSocketState.Aborted)
            {
                client.Abort();
            }
            client.Dispose();
        }

        public async Task DisconnectAsync()
        {
            await _cts.CancelAsync();
            await DisconnectSocketAsync(_client);
        }

        private void OnBroadcastReceived(BroadcastResponseContent broadcast)
        {
            BroadcastReceived?.Invoke(this, broadcast);
        }

        private void OnSubscribeReceived(SubscribeResponseContent subscribe)
        {
            SubscribeReceived?.Invoke(this, subscribe);
        }

        private void OnResponseReceived(ClientResponseContent response)
        {
            ResponseReceived?.Invoke(this, response);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
