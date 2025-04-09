using System.Net.WebSockets;
using System.Text;
using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    internal class WebSocketClient : IWebSocketClient
    {
        private readonly ClientWebSocket _client = new();
        private readonly ClientWebSocket _broadcastClient = new();
        private readonly CancellationTokenSource _cts = new();
        private const int ReconnectIntervalInSeconds = 5;

        public event MessageReceivedEventHandler? MessageReceived;
        public event ClientConnectedEventHandler? ClientConnected;

        public async Task ConnectAsync(string uri, string broadcastUri)
        {
            while (_client.State != WebSocketState.Open)
            {
                try
                {
                    await _client.ConnectAsync(new Uri(uri), CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Blad polaczenia: {ex.Message}. Ponawiam probe za {ReconnectIntervalInSeconds} sekund...");
                    await Task.Delay(ReconnectIntervalInSeconds * 1000);
                }
            }

            while (_broadcastClient.State != WebSocketState.Open)
            {
                try
                {
                    await _broadcastClient.ConnectAsync(new Uri(broadcastUri), CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Blad polaczenia: {ex.Message}. Ponawiam probe za {ReconnectIntervalInSeconds} sekund...");
                    await Task.Delay(ReconnectIntervalInSeconds * 1000);
                }
            }

            ClientConnected?.Invoke(this, ClientConnectedEventArgs.Empty);
            _ = Task.Run(ReceiveLoop);
        }
        public async Task SendAsync(string xml)
        {
            var bytes = Encoding.UTF8.GetBytes(xml);
            await _client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public async Task<string> ReceiveAsync()
        {
            return await Task.Run(async () =>
            {
                var buffer = new byte[4096];
                var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                return result.MessageType != WebSocketMessageType.Binary
                    ? ""
                    : Encoding.UTF8.GetString(buffer, 0, result.Count);
            });
        }

        public async Task<string> SendAndReceiveAsync(string xml)
        {
            return Task.Run(async () =>
            {
                await SendAsync(xml);
                var result = await ReceiveAsync();
                return result;
            }).Result;
        }

        private async Task ReceiveLoop()
        {
            var buffer = new byte[4096];
            while (!_cts.IsCancellationRequested)
            {
                var result = await _broadcastClient.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                if (result.MessageType != WebSocketMessageType.Binary) continue;
                var xml = Encoding.UTF8.GetString(buffer, 0, result.Count);
                OnMessageReceived(this, xml);
            }
        }

        private static async Task DisconnectSocketAsync(ClientWebSocket client)
        {
            if (client.State == WebSocketState.Open || client.State == WebSocketState.CloseReceived)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting",
                    CancellationToken.None);
            } 
            client.Dispose();
        }

        public async Task DisconnectAsync()
        {
            await _cts.CancelAsync();
            await DisconnectSocketAsync(_client);
            await DisconnectSocketAsync(_broadcastClient);
        }

        private void OnMessageReceived(object? source, string xml)
        {
            MessageReceived?.Invoke(source, new MessageReceivedEventArgs(xml));
        }

        public void Dispose()
        {
            _cts.Cancel();
            _client.Dispose();
            _broadcastClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
