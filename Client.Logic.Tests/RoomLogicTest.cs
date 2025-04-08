using TPUM.Client.Data;

namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class RoomLogicTest
    {
        private DataApiBase _dataApi = default!;
        private IRoomLogic _room = default!;
        private const float _width = 100f;
        private const float _height = 100f;

        [TestInitialize]
        public void Setup()
        {
            _dataApi = DataApiBase.GetApi();
            _room = new DummyRoomLogic(_dataApi.AddRoom(_width, _height));
        }

        [TestMethod]
        public void Room_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(_width, _room.Width, 1e-10f);
            Assert.AreEqual(_height, _room.Height, 1e-10f);
            Assert.AreEqual(0, _room.Heaters.Count);
            Assert.AreEqual(0, _room.HeatSensors.Count);
            Assert.AreEqual(0f, _room.RoomTemperature, 1e-10f);
            Assert.AreEqual(0f, _room.AvgTemperature, 1e-10f);
        }

        [TestMethod]
        public void AddHeater_ShouldAddHeaterToRoom()
        {
            IHeaterLogic heater = _room.AddHeater(5f, 5f, 100f);

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
            IHeaterLogic heater = _room.AddHeater(5f, 5f, 100f);

            _room.RemoveHeater(Guid.Empty);

            Assert.AreEqual(1, _room.Heaters.Count);
        }

        [TestMethod]
        public void RemoveHeater_ShouldRemoveHeaterFromRoom()
        {
            IHeaterLogic heater = _room.AddHeater(5f, 5f, 100f);

            _room.RemoveHeater(heater.Id);

            Assert.AreEqual(0, _room.Heaters.Count);
        }

        [TestMethod]
        public void AddHeatSensor_ShouldAddSensorToRoom()
        {
            IHeatSensorLogic sensor = _room.AddHeatSensor(5f, 5f);

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
            IHeatSensorLogic sensor = _room.AddHeatSensor(5f, 5f);
            ((DummyHeatSensorLogic)sensor).SetTemperature(25f);

            float temp = ((DummyRoomLogic)_room).GetTemperatureAtPosition(5f, 5f);
            Assert.AreEqual(25f, temp);
        }

        [TestMethod]
        public void GetTemperatureAtPosition_ShouldReturnCorrectTemperature_WithMultipleSensors()
        {
            IHeatSensorLogic sensor1 = _room.AddHeatSensor(5f, 5f);
            ((DummyHeatSensorLogic)sensor1).SetTemperature(25f);

            IHeatSensorLogic sensor2 = _room.AddHeatSensor(5f, 7f);
            ((DummyHeatSensorLogic)sensor2).SetTemperature(35f);

            float temp = _room.GetTemperatureAtPosition(5f, 6f);
            Assert.AreEqual(30f, temp);
        }

        [TestMethod]
        public void GetTemperatureAtPosition_ShouldWeightTemperatureByDistance()
        {
            IHeatSensorLogic sensor1 = _room.AddHeatSensor(2f, 2f);
            ((DummyHeatSensorLogic)sensor1).SetTemperature(40f);

            IHeatSensorLogic sensor2 = _room.AddHeatSensor(8f, 8f);
            ((DummyHeatSensorLogic)sensor2).SetTemperature(20f);

            float temp = _room.GetTemperatureAtPosition(6f, 6f);

            Assert.IsTrue(temp < 30f && temp > 20f, $"Expected temperature between 20 and 30, but got {temp}");
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldNotRemoveHeatSensorFromRoomIfIdNotFound()
        {
            IHeatSensorLogic sensor = _room.AddHeatSensor(5f, 5f);

            _room.RemoveHeatSensor(Guid.Empty);

            Assert.AreEqual(1, _room.HeatSensors.Count);
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldRemoveHeatSensorFromRoom()
        {
            IHeatSensorLogic sensor = _room.AddHeatSensor(5f, 5f);

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
            IHeatSensorLogic sensor1 = _room.AddHeatSensor(3f, 3f);
            ((DummyHeatSensorLogic)sensor1).SetTemperature(20f);
            IHeatSensorLogic sensor2 = _room.AddHeatSensor(7f, 7f);
            ((DummyHeatSensorLogic)sensor2).SetTemperature(30f);

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

        [TestMethod]
        public void UpdateTemperature_ShouldUpdateSensorTemperaturesBasedOnHeater()
        {
            IHeaterLogic heater = _room.AddHeater(5f, 5f, 100f);
            heater.TurnOn();

            IHeatSensorLogic sensor1 = _room.AddHeatSensor(3f, 3f);
            ((DummyHeatSensorLogic)sensor1).SetTemperature(20f);
            IHeatSensorLogic sensor2 = _room.AddHeatSensor(7f, 7f);
            ((DummyHeatSensorLogic)sensor2).SetTemperature(20f);

            float initialTemp1 = sensor1.Temperature;
            float initialTemp2 = sensor2.Temperature;

            ((DummyRoomLogic)_room).UpdateTemperature(200);

            Assert.IsTrue(sensor1.Temperature > initialTemp1, $"Expected sensor1 temperature to increase from {initialTemp1}, but got {sensor1.Temperature}");
            Assert.IsTrue(sensor2.Temperature > initialTemp2, $"Expected sensor2 temperature to increase from {initialTemp2}, but got {sensor2.Temperature}");
        }
    };
}
