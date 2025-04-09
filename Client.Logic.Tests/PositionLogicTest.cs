namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class PositionLogicTest
    {
        private IPositionLogic _position = default!;
        private const float _x = 2f;
        private const float _y = 2f;

        [TestInitialize]
        public void Setup()
        {
            _position = new DummyPositionLogic(new TestPositionData { X = _x, Y = _y });
        }

        [TestMethod]
        public void Position_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(_x, _position.X, 1e-10f);
            Assert.AreEqual(_y, _position.Y, 1e-10f);
        }

        [TestMethod]
        public void XProperty_ShouldUpdateCorrectly()
        {
            _position.X += 1f;
            Assert.AreEqual(_x + 1f, _position.X, 1e-10f);
        }

        [TestMethod]
        public void YProperty_ShouldUpdateCorrectly()
        {
            _position.Y += 1f;
            Assert.AreEqual(_y + 1f, _position.Y, 1e-10f);
        }

        [TestMethod]
        public void PositionLogic_ShouldUpdateXandY()
        {
            var data = new TestPositionData { X = 1.0f, Y = 2.0f };
            var logic = new DummyPositionLogic(data);

            logic.X = 3.0f;
            logic.Y = 4.0f;

            Assert.AreEqual(3.0f, logic.X);
            Assert.AreEqual(4.0f, logic.Y);
        }
    }
}
