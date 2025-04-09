using System.Net.WebSockets;
using System.Text;
using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    internal class WebSocketClient : IWebSocketClient
    {
        private readonly ClientWebSocket _client = new();
        private readonly CancellationTokenSource _cts = new();

        public event MessageReceivedEventHandler? MessageReceived;

        public async Task ConnectAsync(string uri)
        {
            await _client.ConnectAsync(new Uri(uri), CancellationToken.None);
            _ = ReceiveLoop();
        }
        public async Task SendAsync(string xml)
        {
            var bytes = Encoding.UTF8.GetBytes(xml);
            await _client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<string> ReceiveAsync()
        {
            var buffer = new byte[4096];
            var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        public async Task<string> SendAndReceiveAsync(string xml)
        {
            await SendAsync(xml);
            var result = await ReceiveAsync();
            return result;
        }

        private async Task ReceiveLoop()
        {
            var buffer = new byte[4096];
            while (!_cts.IsCancellationRequested)
            {
                var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var xml = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    OnMessageReceived(this, xml);
                }
            }
        }

        public async Task DisconnectAsync()
        {
            if (_client == null) return;
            _cts.Cancel();
            if (_client.State == WebSocketState.Open || _client.State == WebSocketState.CloseReceived)
            {
                await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
            }

            _client.Dispose();
        }

        private void OnMessageReceived(object? source, string xml)
        {
            MessageReceived?.Invoke(source, new MessageReceivedEventArgs(xml));
        }
    }
}
