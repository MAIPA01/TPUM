namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class LogicAPITest
    {
        private LogicApiBase? _api = null;

        [TestInitialize]
        public void Setup()
        {
            _api = LogicApiBase.GetApi("ws://localhost:5000/ws");
        }

        [TestMethod]
        public void LogicApi_CanGet()
        {
            Assert.IsNotNull(_api);
        }

        [TestMethod]
        public void LogicApi_Dispose()
        {
            _api!.Dispose();
        }
    }
}