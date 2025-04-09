using Server.Presentation;

namespace TPUM.Server.Presentation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Server starting...");
            var server = new WebSocketServer("http://localhost:5000/ws/");
            await server.StartAsync();
        }
    }
}