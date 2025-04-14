namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class PositionLogicTest
    {
        private IPositionLogic _position = default!;

        [TestInitialize]
        public void Setup()
        {
            _position = new DummyPositionLogic(new TestPositionData());
        }

        [TestMethod]
        public void Position_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(0f, _position.X, 1e-10f);
            Assert.AreEqual(0f, _position.Y, 1e-10f);
        }
    }
}