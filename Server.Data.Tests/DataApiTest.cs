namespace TPUM.Server.Data.Tests
{
    [TestClass]
    public sealed class DataApiTest
    {
        private DataApiBase _dataApi = default!;

        [TestInitialize]
        public void Setup()
        {
            _dataApi = DataApiBase.GetApi();
        }

        [TestMethod]
        public void Initialized_ShouldNotThrowException()
        {
            Assert.IsNotNull(_dataApi);
        }

        [TestMethod]
        public void Dispose_ShouldNotThrowException()
        {
            _dataApi.Dispose();
        }
    }
}