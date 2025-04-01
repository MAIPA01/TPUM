namespace TPUM.Logic.Tests
{
    [TestClass]
    public sealed class RoomTest
    {
        private Room _room;
        private const long _id = 10;
        private const float _width = 100f;
        private const float _height = 100f;

        [TestInitialize]
        public void Setup()
        {
            _room = new Room(_id, _width, _height, null);
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
            var heater = _room.AddHeater(5f, 5f, 100f);

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
            var heater = _room.AddHeater(5f, 5f, 100f);

            _room.RemoveHeater(1);

            Assert.AreEqual(1, _room.Heaters.Count);
        }

        [TestMethod]
        public void RemoveHeater_ShouldRemoveHeaterFromRoom()
        {
            var heater = _room.AddHeater(5f, 5f, 100f);

            _room.RemoveHeater(heater.Id);

            Assert.AreEqual(0, _room.Heaters.Count);
        }

        [TestMethod]
        public void AddHeatSensor_ShouldAddSensorToRoom()
        {
            var sensor = _room.AddHeatSensor(5f, 5f);

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
            var sensor = _room.AddHeatSensor(5f, 5f);
            sensor.Temperature = 25f;

            float temp = _room.GetTemperatureAtPosition(5f, 5f);
            Assert.AreEqual(25f, temp);
        }

        [TestMethod]
        public void GetTemperatureAtPosition_ShouldReturnCorrectTemperature_WithMultipleSensors()
        {
            var sensor1 = _room.AddHeatSensor(5f, 5f);
            sensor1.Temperature = 25f;

            var sensor2 = _room.AddHeatSensor(5f, 5f);
            sensor2.Temperature = 35f;

            float temp = _room.GetTemperatureAtPosition(5f, 5f);
            Assert.AreEqual(30f, temp);
        }

        [TestMethod]
        public void GetTemperatureAtPosition_ShouldWeightTemperatureByDistance()
        {
            var sensor1 = _room.AddHeatSensor(2f, 2f);
            sensor1.Temperature = 40f;

            var sensor2 = _room.AddHeatSensor(8f, 8f);
            sensor2.Temperature = 20f;

            float temp = _room.GetTemperatureAtPosition(6f, 6f);

            Assert.IsTrue(temp < 40f && temp > 30f, $"Expected temperature between 30 and 40, but got {temp}");
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldNotRemoveHeatSensorFromRoomIfIdNotFound()
        {
            var sensor = _room.AddHeatSensor(5f, 5f);

            _room.RemoveHeatSensor(1);

            Assert.AreEqual(1, _room.HeatSensors.Count);
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldRemoveHeatSensorFromRoom()
        {
            var sensor = _room.AddHeatSensor(5f, 5f);

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
            var sensor1 = _room.AddHeatSensor(3f, 3f);
            sensor1.Temperature = 20f;
            var sensor2 = _room.AddHeatSensor(7f, 7f);
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

            _room.StartSimulation();

            _room.Dispose();

            Assert.AreEqual(0, _room.HeatSensors.Count);
            Assert.AreEqual(0, _room.Heaters.Count);
        }

        [TestMethod]
        public void UpdateTemperature_ShouldUpdateSensorTemperaturesBasedOnHeater()
        {
            var heater = _room.AddHeater(5f, 5f, 100f);
            heater.TurnOn();

            var sensor1 = _room.AddHeatSensor(3f, 3f);
            sensor1.Temperature = 20f;
            var sensor2 = _room.AddHeatSensor(7f, 7f);
            sensor2.Temperature = 20f;

            _room.StartSimulation();

            Thread.Sleep(20);
            _room.EndSimulation();

            Assert.IsTrue(sensor1.Temperature > 20f, $"Expected sensor1 temperature to be greater than 20, but got {sensor1.Temperature}");
            Assert.IsTrue(sensor2.Temperature > 20f, $"Expected sensor2 temperature to be greater than 20, but got {sensor2.Temperature}");
        }
    }
}
