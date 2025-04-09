namespace TPUM.Client.Data
{
    public interface IWebSocketClient
    {
        Task ConnectAsync(string uri);

        Task SendAsync(string xml);

        Task<string> ReceiveAsync();

        Task<string> SendAndReceiveAsync(string xml);

        Task DisconnectAsync();
    }
}
