using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Collections.Concurrent;

namespace Server.Presentation
{
    internal class WebSocketServer
    {
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
                    Console.WriteLine($"📨 Received from {clientId}: {message}");

                    var response = Encoding.UTF8.GetBytes($"🔁 Echo: {message}");
                    await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);
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

        public async Task BroadcastAsync(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);

            foreach (var (clientId, socket) in _clients)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}