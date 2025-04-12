namespace TPUM.Server.Data.Tests
{
    [TestClass]
    public sealed class HeatSensorDataTest
    {
        private IHeatSensorData? _sensor = null;
        private static readonly Guid Id = Guid.Parse("44465E20-25A1-4FA3-BE1D-0AB7E5C739F4");
        private const float X = 2f;
        private const float Y = 2f;

        [TestInitialize]
        public void Setup()
        {
            _sensor = new DummyHeatSensorData(Id, X, Y);
        }

        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            Assert.AreEqual(Id, _sensor!.Id);
            Assert.AreEqual(X, _sensor!.Position.X);
            Assert.AreEqual(Y, _sensor!.Position.Y);
            Assert.AreEqual(0f, _sensor!.Temperature, 1e-10f);
        }

        [TestMethod]
        public void SetPosition_ShouldUpdatePositionCorrectly()
        {
            Assert.AreEqual(X, _sensor!.Position.X);
            Assert.AreEqual(Y, _sensor!.Position.Y);
            _sensor.SetPosition(X + 1f, Y + 1f);
            Assert.AreEqual(X + 1f, _sensor!.Position.X);
            Assert.AreEqual(Y + 1f, _sensor!.Position.Y);
        }

        [TestMethod]
        public void Temperature_ShouldReturnCorrectValue()
        {
            Assert.AreEqual(0f, _sensor!.Temperature, 1e-10f);
        }

        [TestMethod]
        public void SetTemperature_ShouldUpdateTemperatureCorrectly()
        {
            Assert.AreEqual(0f, _sensor!.Temperature, 1e-10f);
            _sensor.Temperature = 1f;
            Assert.AreEqual(1f, _sensor!.Temperature, 1e-10f);
        }

        [TestMethod]
        public void HeatSensorData_TemperatureChangeEvent_IsTriggered()
        {
            var sensor = new DummyHeatSensorData(Guid.NewGuid(), 1f, 1f);
            var eventTriggered = false;

            sensor.TemperatureChanged += (s, oldT, newT) => eventTriggered = true;

            sensor.Temperature = 20.5f;
            Assert.IsTrue(eventTriggered);
        }
    }
}