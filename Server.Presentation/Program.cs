namespace TPUM.Server.Presentation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var api = PresentationApiBase.GetApi("http://localhost:5000/", "http://localhost:5050/");
            Console.WriteLine("Server starting...");
            await api.StartServer();
        }
    }
}