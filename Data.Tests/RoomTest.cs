namespace TPUM.Data.Tests
{
    [TestClass]
    public sealed class RoomTest
    {
        private IRoom _room = default!;
        private const long _id = 10;
        private const float _width = 100f;
        private const float _height = 100f;

        [TestInitialize]
        public void Setup()
        {
            _room = new DummyRoom(_id, _width, _height);
        }

        [TestMethod]
        public void Room_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(_id, _room.Id);
            Assert.AreEqual(_width, _room.Width, 1e-10f);
            Assert.AreEqual(_height, _room.Height, 1e-10f);
            Assert.AreEqual(0, _room.Heaters.Count);
            Assert.AreEqual(0, _room.HeatSensors.Count);
            Assert.AreEqual(0f, _room.AvgTemperature, 1e-10f);
        }

        [TestMethod]
        public void AddHeater_ShouldAddHeaterToRoom()
        {
            IHeater heater = _room.AddHeater(5f, 5f, 100f);

            Assert.IsNotNull(heater);
            Assert.AreEqual(1, _room.Heaters.Count);
            Assert.AreSame(heater, _room.Heaters.First());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddHeater_ShouldThrowException_WhenPositionOutOfRange()
        {
            _room.AddHeater(110f, 5f, 100f);
        }

        [TestMethod]
        public void RemoveHeater_ShouldNotRemoveHeaterFromRoomIfIdNotFound()
        {
            IHeater heater = _room.AddHeater(5f, 5f, 100f);

            _room.RemoveHeater(1);

            Assert.AreEqual(1, _room.Heaters.Count);
        }

        [TestMethod]
        public void RemoveHeater_ShouldRemoveHeaterFromRoom()
        {
            IHeater heater = _room.AddHeater(5f, 5f, 100f);

            _room.RemoveHeater(heater.Id);

            Assert.AreEqual(0, _room.Heaters.Count);
        }

        [TestMethod]
        public void AddHeatSensor_ShouldAddSensorToRoom()
        {
            IHeatSensor sensor = _room.AddHeatSensor(5f, 5f);

            Assert.IsNotNull(sensor);
            Assert.AreEqual(1, _room.HeatSensors.Count);
            Assert.AreSame(sensor, _room.HeatSensors.First());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddHeatSensor_ShouldThrowException_WhenPositionOutOfRange()
        {
            _room.AddHeatSensor(110f, 5f);
        }

        [TestMethod]
        public void GetTemperatureAtPosition_ShouldReturnCorrectTemperature()
        {
            IHeatSensor sensor = _room.AddHeatSensor(5f, 5f);
            sensor.Temperature = 25f;

            float temp = _room.GetTemperatureAtPosition(5f, 5f);
            Assert.AreEqual(25f, temp);
        }

        [TestMethod]
        public void GetTemperatureAtPosition_ShouldReturnCorrectTemperature_WithMultipleSensors()
        {
            IHeatSensor sensor1 = _room.AddHeatSensor(5f, 5f);
            sensor1.Temperature = 25f;

            IHeatSensor sensor2 = _room.AddHeatSensor(5f, 7f);
            sensor2.Temperature = 35f;

            float temp = _room.GetTemperatureAtPosition(5f, 6f);
            Assert.AreEqual(30f, temp);
        }

        [TestMethod]
        public void GetTemperatureAtPosition_ShouldWeightTemperatureByDistance()
        {
            IHeatSensor sensor1 = _room.AddHeatSensor(2f, 2f);
            sensor1.Temperature = 40f;

            IHeatSensor sensor2 = _room.AddHeatSensor(8f, 8f);
            sensor2.Temperature = 20f;

            float temp = _room.GetTemperatureAtPosition(6f, 6f);

            Assert.IsTrue(temp < 30f && temp > 20f, $"Expected temperature between 20 and 30, but got {temp}");
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldNotRemoveHeatSensorFromRoomIfIdNotFound()
        {
            IHeatSensor sensor = _room.AddHeatSensor(5f, 5f);

            _room.RemoveHeatSensor(1);

            Assert.AreEqual(1, _room.HeatSensors.Count);
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldRemoveHeatSensorFromRoom()
        {
            IHeatSensor sensor = _room.AddHeatSensor(5f, 5f);

            _room.RemoveHeatSensor(sensor.Id);

            Assert.AreEqual(0, _room.HeatSensors.Count);
        }

        [TestMethod]
        public void AvgTemperature_ShouldReturnZero_WhenNoSensors()
        {
            Assert.AreEqual(0f, _room.AvgTemperature);
        }

        [TestMethod]
        public void AvgTemperature_ShouldCalculateCorrectly()
        {
            IHeatSensor sensor1 = _room.AddHeatSensor(3f, 3f);
            sensor1.Temperature = 20f;
            IHeatSensor sensor2 = _room.AddHeatSensor(7f, 7f);
            sensor2.Temperature = 30f;

            Assert.AreEqual(25f, _room.AvgTemperature);
        }

        [TestMethod]
        public void ClearHeaters_ShouldRemoveAllHeaters()
        {
            _room.AddHeater(5f, 5f, 100f);
            Assert.AreEqual(1, _room.Heaters.Count);

            _room.ClearHeaters();
            Assert.AreEqual(0, _room.Heaters.Count);
        }

        [TestMethod]
        public void ClearHeatSensors_ShouldRemoveAllSensors()
        {
            _room.AddHeatSensor(5f, 5f);
            Assert.AreEqual(1, _room.HeatSensors.Count);

            _room.ClearHeatSensors();
            Assert.AreEqual(0, _room.HeatSensors.Count);
        }

        [TestMethod]
        public void Dispose_ShouldClearHeatSensorsAndHeatersAfterDisposing()
        {
            _room.AddHeatSensor(5f, 5f);
            Assert.AreEqual(1, _room.HeatSensors.Count);
            _room.AddHeater(5f, 5f, 100f);
            Assert.AreEqual(1, _room.Heaters.Count);

            _room.Dispose();

            Assert.AreEqual(0, _room.HeatSensors.Count);
            Assert.AreEqual(0, _room.Heaters.Count);
        }
    }
}