namespace TPUM.Client.Data.Tests
{
    [TestClass]
    public sealed class DataApiTest
    {
        private DataApiBase _api = default!;

        [TestInitialize]
        public void Setup()
        {
            _api = DataApiBase.GetApi("ws://localhost:5000/ws");
        }

        [TestMethod]
        public void DataApi_CanGet()
        {
            Assert.IsNotNull(_api);
        }

        [TestMethod]
        public void DataApi_Dispose()
        {
            _api.Dispose();
        }
    }
}