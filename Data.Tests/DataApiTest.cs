namespace TPUM.Data.Tests
{
    [TestClass]
    public sealed class DataApiTest
    {
        private DataApi _dataApi;

        [TestInitialize]
        public void Setup()
        {
            _dataApi = new DataApi();
        }

        [TestMethod]
        public void CreateHeater_ShouldReturnValidHeater()
        {
            float x = 10.0f, y = 20.0f, temperature = 100.0f;

            var heater = _dataApi.CreateHeater(x, y, temperature);

            Assert.IsNotNull(heater);
            Assert.AreEqual(x, heater.Position.X, 1e-10f);
            Assert.AreEqual(y, heater.Position.Y, 1e-10f);
            Assert.AreEqual(0f, heater.Temperature, 1e-10f);
            heater.TurnOn();
            Assert.AreEqual(temperature, heater.Temperature, 1e-10f);
        }

        [TestMethod]
        public void CreateHeatSensor_ShouldReturnValidHeatSensor()
        {
            float x = 15.0f, y = 25.0f;

            var sensor = _dataApi.CreateHeatSensor(x, y);

            Assert.IsNotNull(sensor);
            Assert.AreEqual(x, sensor.Position.X, 1e-10f);
            Assert.AreEqual(y, sensor.Position.Y, 1e-10f);
            Assert.AreEqual(0f, sensor.Temperature, 1e-10f);
        }

        [TestMethod]
        public void CreatePosition_ShouldReturnValidPosition()
        {
            float x = 5.0f, y = 10.0f;

            var position = _dataApi.CreatePosition(x, y);

            Assert.IsNotNull(position);
            Assert.AreEqual(x, position.X, 1e-10f);
            Assert.AreEqual(y, position.Y, 1e-10f);
        }

        [TestMethod]
        public void Dispose_ShouldNotThrowException()
        {
            _dataApi.Dispose();
        }
    }
}