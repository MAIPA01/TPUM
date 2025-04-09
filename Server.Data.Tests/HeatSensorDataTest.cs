namespace TPUM.Server.Data.Tests
{
    [TestClass]
    public sealed class HeatSensorDataTest
    {
        private IHeatSensorData _sensor = default!;
        private static readonly Guid _id = Guid.Parse("44465E20-25A1-4FA3-BE1D-0AB7E5C739F4");
        private const float _x = 2f;
        private const float _y = 2f;
        private IPositionData _position = default!;

        [TestInitialize]
        public void Setup()
        {
            _position = new DummyPositionData(_x, _y);
            _sensor = new DummyHeatSensorData(_id, _position);
        }

        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            Assert.AreEqual(_id, _sensor.Id);
            Assert.AreEqual(_position, _sensor.Position);
            Assert.AreEqual(_x, _sensor.Position.X);
            Assert.AreEqual(_y, _sensor.Position.Y);
            Assert.AreEqual(0f, _sensor.Temperature, 1e-10f);
        }

        [TestMethod]
        public void SetPosition_ShouldUpdatePositionCorrectly()
        {
            Assert.AreEqual(_position, _sensor.Position);
            _sensor.Position.X += 1f;
            _position.X += 1f;
            Assert.AreEqual(_position, _sensor.Position);
        }

        [TestMethod]
        public void Temperature_ShouldReturnCorrectValue()
        {
            Assert.AreEqual(0f, _sensor.Temperature, 1e-10f);
        }

        [TestMethod]
        public void SetTemperature_ShouldUpdateTemperatureCorrectly()
        {
            Assert.AreEqual(0f, _sensor.Temperature, 1e-10f);
            _sensor.Temperature = 1f;
            Assert.AreEqual(1f, _sensor.Temperature, 1e-10f);
        }
    }
}