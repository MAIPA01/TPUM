namespace TPUM.Server.Data.Tests
{
    [TestClass]
    public sealed class PositionDataTest
    {
        private IPositionData? _position = null;
        private const float X = 2f;
        private const float Y = 2f;

        [TestInitialize]
        public void Setup()
        {
            _position = new DummyPositionData(X, Y);
        }

        [TestMethod]
        public void Position_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(X, _position!.X, 1e-10f);
            Assert.AreEqual(Y, _position!.Y, 1e-10f);
        }
    }
}