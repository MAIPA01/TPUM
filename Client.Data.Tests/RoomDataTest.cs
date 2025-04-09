namespace TPUM.Client.Data.Tests
{
    [TestClass]
    public sealed class RoomDataTest
    {
        private IRoomData _room = default!;
        private static readonly Guid _id = Guid.Parse("44465E20-25A1-4FA3-BE1D-0AB7E5C739F4");
        private static readonly Guid Test = Guid.Parse("44465E20-25A1-4FA3-BE1D-0AB7E5C73901");
        private const string _name = "name";
        private const float _width = 100f;
        private const float _height = 100f;

        [TestInitialize]
        public void Setup()
        {
            _room = new DummyRoomData(_id, _name, _width, _height);
        }

        [TestMethod]
        public void Room_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(_id, _room.Id);
            Assert.AreEqual(_name, _room.Name);
            Assert.AreEqual(_width, _room.Width, 1e-10f);
            Assert.AreEqual(_height, _room.Height, 1e-10f);
            Assert.AreEqual(0, _room.Heaters.Count);
            Assert.AreEqual(0, _room.HeatSensors.Count);
        }

        [TestMethod]
        public void AddHeater_ShouldAddHeaterToRoom()
        {
            IHeaterData toAdd = new DummyHeaterData(Test, new DummyPositionData(5f, 5f), 100f);
            IHeaterData heater = _room.AddHeater(toAdd);

            Assert.IsNotNull(heater);
            Assert.AreEqual(1, _room.Heaters.Count);
            Assert.AreSame(heater, _room.Heaters.First());
            Assert.AreEqual(toAdd.Id, heater.Id);
            Assert.AreEqual(toAdd.Temperature, heater.Temperature);
            Assert.AreEqual(toAdd.Position, heater.Position);
            Assert.AreEqual(toAdd.IsOn, heater.IsOn);
        }

        [TestMethod]
        public void AddHeater_ShouldNotThrowException_WhenPositionOutOfRange()
        {
            _room.AddHeater(new DummyHeaterData(Test, new DummyPositionData(5f, 5f), 100f));
        }

        [TestMethod]
        public void RemoveHeater_ShouldNotRemoveHeaterFromRoomIfIdNotFound()
        {
            _room.AddHeater(new DummyHeaterData(Test, new DummyPositionData(5f, 5f), 100f));

            _room.RemoveHeater(Guid.Empty);

            Assert.AreEqual(1, _room.Heaters.Count);
        }

        [TestMethod]
        public void RemoveHeater_ShouldRemoveHeaterFromRoom()
        {
            IHeaterData heater = _room.AddHeater(new DummyHeaterData(Test, new DummyPositionData(5f, 5f), 100f));

            _room.RemoveHeater(heater.Id);

            Assert.AreEqual(0, _room.Heaters.Count);
        }

        [TestMethod]
        public void AddHeatSensor_ShouldAddSensorToRoom()
        {
            IHeatSensorData toAdd = new DummyHeatSensorData(Test, new DummyPositionData(5f, 5f));
            IHeatSensorData sensor = _room.AddHeatSensor(toAdd);

            Assert.IsNotNull(sensor);
            Assert.AreEqual(1, _room.HeatSensors.Count);
            Assert.AreSame(sensor, _room.HeatSensors.First());
            Assert.AreEqual(toAdd.Id, sensor.Id);
            Assert.AreEqual(toAdd.Temperature, sensor.Temperature);
            Assert.AreEqual(toAdd.Position, sensor.Position);
        }

        [TestMethod]
        public void AddHeatSensor_ShouldNotThrowException_WhenPositionOutOfRange()
        {
            _room.AddHeatSensor(new DummyHeatSensorData(Test, new DummyPositionData(110f, 5f)));
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldNotRemoveHeatSensorFromRoomIfIdNotFound()
        {
            _room.AddHeatSensor(new DummyHeatSensorData(Test, new DummyPositionData(5f, 5f)));

            _room.RemoveHeatSensor(Guid.Empty);

            Assert.AreEqual(1, _room.HeatSensors.Count);
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldRemoveHeatSensorFromRoom()
        {
            IHeatSensorData sensor = _room.AddHeatSensor(new DummyHeatSensorData(Test, new DummyPositionData(5f, 5f)));

            _room.RemoveHeatSensor(sensor.Id);

            Assert.AreEqual(0, _room.HeatSensors.Count);
        }
    }
}