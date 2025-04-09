namespace TPUM.Client.Presentation.Model.Tests
{
    [TestClass]
    public sealed class ModelApiTest
    {
        private ModelApiBase _modelApi = default!;

        [TestInitialize]
        public void Setup()
        {
            _modelApi = ModelApiBase.GetApi("ws://localhost:5000/ws");
        }

        [TestMethod]
        public void ModelApi_ShouldInitialize_WithCorrectProperties()
        {
            Assert.IsNotNull(_modelApi);
            Assert.AreEqual(0, _modelApi.Rooms.Count);
        }

        [TestMethod]
        public void ModelApi_Dispose()
        {
            _modelApi.Dispose();
            Assert.AreEqual(0, _modelApi.Rooms.Count);
        }
    }
}
