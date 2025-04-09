using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Collections.Concurrent;
using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    internal class WebSocketServer : INotifyOnClientMessageReceived
    {
        public event ClientMessageReceivedEventHandler? ClientMessageReceived;

        private readonly HttpListener _httpListener = new();
        private readonly string _uriPrefix;

        private readonly ConcurrentDictionary<Guid, WebSocket> _clients = new();

        public WebSocketServer(string uriPrefix)
        {
            _uriPrefix = uriPrefix;
        }

        public async Task StartAsync()
        {
            _httpListener.Prefixes.Add(_uriPrefix);
            _httpListener.Start();

            Console.WriteLine($"✅ WebSocket Server started on {_uriPrefix}");

            while (true)
            {
                var context = await _httpListener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    var clientId = Guid.NewGuid();

                    _clients.TryAdd(clientId, wsContext.WebSocket);

                    Console.WriteLine($"⚫ Client {clientId} connected.");

                    // Obsługa klienta w tle
                    _ = Task.Run(() => HandleClientAsync(clientId, wsContext.WebSocket));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async Task HandleClientAsync(Guid clientId, WebSocket webSocket)
        {
            var buffer = new byte[4096];
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"📨 Received from {clientId}: \n{message}");

                    OnClientMessageReceived(clientId, message);
                }
            }
            catch (WebSocketException wse)
            {
                Console.WriteLine($"⚠️ Client {clientId} error: {wse.Message}");
            }
            finally
            {
                // Czyszczenie połączenia
                if (_clients.TryRemove(clientId, out _))
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye!", CancellationToken.None);
                    webSocket.Dispose();
                    Console.WriteLine($"❌ Client {clientId} disconnected.");
                }
            }
        }

        public async Task SendAsync(Guid clientId, string message)
        {
            if (!_clients.TryGetValue(clientId, out var socket))
            {
                Console.WriteLine($"⚠️ Client {clientId} could not be found.");
                return;
            }

            //var data = Encoding.UTF8.GetBytes(message);
            //await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None);

            var data = Encoding.UTF8.GetBytes("HII");
            await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None);
            Console.WriteLine("D");
        }

        public async Task BroadcastAsync(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);

            foreach (var (clientId, socket) in _clients)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None);
                }
            }
        }

        private void OnClientMessageReceived(Guid clientId, string message)
        {
            ClientMessageReceived?.Invoke(this, clientId, message);
        }
    }
}