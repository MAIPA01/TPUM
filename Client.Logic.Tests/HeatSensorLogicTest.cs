namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class HeatSensorLogicTest
    {
        private IHeatSensorLogic? _sensor = null;

        [TestInitialize]
        public void Setup()
        {
            _sensor = new DummyHeatSensorLogic(new TestHeatSensorData());
        }

        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            Assert.AreEqual(0f, _sensor!.Position.X);
            Assert.AreEqual(0f, _sensor!.Position.Y);
            Assert.AreEqual(0f, _sensor.Temperature, 1e-10f);
        }

        [TestMethod]
        public void HeatSensorLogic_ShouldReturnCorrectTemperature()
        {
            var data = new TestHeatSensorData();
            var logic = new DummyHeatSensorLogic(data);

            Assert.AreEqual(0f, logic.Temperature);
        }

        [TestMethod]
        public void HashCode_ShouldReturnConsistentValue()
        {
            var hash = 3 * _sensor!.Position.GetHashCode() + 5 * _sensor!.Temperature.GetHashCode();
            Assert.AreEqual(hash, _sensor.GetHashCode(), 1);
        }
    }
}