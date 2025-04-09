namespace TPUM.Server.Data.Tests
{
    [TestClass]
    public sealed class PositionDataTest
    {
        private IPositionData _position = default!;
        private const float _x = 2f;
        private const float _y = 2f;

        [TestInitialize]
        public void Setup()
        {
            _position = new DummyPositionData(_x, _y);
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
        public void PositionData_PositionChangeEvent_IsTriggered()
        {
            var position = new DummyPositionData(0, 0);
            bool eventTriggered = false;

            position.PositionChanged += (s, old, newPos) => eventTriggered = true;

            position.X = 10;
            Assert.IsTrue(eventTriggered);
        }
    }
}