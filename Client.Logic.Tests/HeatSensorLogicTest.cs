namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class HeatSensorLogicTest
    {
        private IHeatSensorLogic _sensor = default!;
        private const float _x = 2f;
        private const float _y = 2f;

        [TestInitialize]
        public void Setup()
        {
            _sensor = new DummyHeatSensorLogic(new TestHeatSensorData
            {
                Position = new TestPositionData { X = _x, Y = _y },
                Temperature = 18.3f
            });
        }

        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            Assert.AreEqual(_x, _sensor.Position.X);
            Assert.AreEqual(_y, _sensor.Position.Y);
            Assert.AreEqual(18.3f, _sensor.Temperature, 1e-10f);
        }

        [TestMethod]
        public void HeatSensorLogic_ShouldReturnCorrectTemperature()
        {
            var data = new TestHeatSensorData
            {
                Temperature = 18.3f
            };
            var logic = new DummyHeatSensorLogic(data);

            Assert.AreEqual(18.3f, logic.Temperature);
        }

        [TestMethod]
        public void HashCode_ShouldReturnConsistentValue()
        {
            var hash = 3 * _sensor.Position.GetHashCode() + 5 * _sensor.Temperature.GetHashCode();
            Assert.AreEqual(hash, _sensor.GetHashCode(), 1);
        }
    }
}