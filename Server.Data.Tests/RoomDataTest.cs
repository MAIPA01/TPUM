namespace TPUM.Server.Data.Tests
{
    [TestClass]
    public sealed class RoomDataTest
    {
        private IRoomData _room = default!;
        private static readonly Guid _id = Guid.Parse("44465E20-25A1-4FA3-BE1D-0AB7E5C739F4");
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
            IHeaterData heater = _room.AddHeater(5f, 5f, 100f);

            Assert.IsNotNull(heater);
            Assert.AreEqual(1, _room.Heaters.Count);
            Assert.AreSame(heater, _room.Heaters.First());
        }

        [TestMethod]
        public void AddHeater_ShouldNotThrowException_WhenPositionOutOfRange()
        {
            _room.AddHeater(110f, 5f, 100f);
        }

        [TestMethod]
        public void RemoveHeater_ShouldNotRemoveHeaterFromRoomIfIdNotFound()
        {
            _room.AddHeater(5f, 5f, 100f);

            _room.RemoveHeater(Guid.Empty);

            Assert.AreEqual(1, _room.Heaters.Count);
        }

        [TestMethod]
        public void RemoveHeater_ShouldRemoveHeaterFromRoom()
        {
            IHeaterData heater = _room.AddHeater(5f, 5f, 100f);

            _room.RemoveHeater(heater.Id);

            Assert.AreEqual(0, _room.Heaters.Count);
        }

        [TestMethod]
        public void AddHeatSensor_ShouldAddSensorToRoom()
        {
            IHeatSensorData sensor = _room.AddHeatSensor(5f, 5f);

            Assert.IsNotNull(sensor);
            Assert.AreEqual(1, _room.HeatSensors.Count);
            Assert.AreSame(sensor, _room.HeatSensors.First());
        }

        [TestMethod]
        public void AddHeatSensor_ShouldNotThrowException_WhenPositionOutOfRange()
        {
            _room.AddHeatSensor(110f, 5f);
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldNotRemoveHeatSensorFromRoomIfIdNotFound()
        {
            _room.AddHeatSensor(5f, 5f);

            _room.RemoveHeatSensor(Guid.Empty);

            Assert.AreEqual(1, _room.HeatSensors.Count);
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldRemoveHeatSensorFromRoom()
        {
            IHeatSensorData sensor = _room.AddHeatSensor(5f, 5f);

            _room.RemoveHeatSensor(sensor.Id);

            Assert.AreEqual(0, _room.HeatSensors.Count);
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
        public void RoomData_AddHeater_IncreasesHeaterCount()
        {
            var room = new DummyRoomData(Guid.NewGuid(), "TestRoom", 10, 10);

            var heater = room.AddHeater(1, 2, 21.5f);

            Assert.AreEqual(1, room.Heaters.Count);
            Assert.IsTrue(room.ContainsHeater(heater.Id));
        }

        [TestMethod]
        public void RoomData_RemoveHeater_DecreasesHeaterCount()
        {
            var room = new DummyRoomData(Guid.NewGuid(), "TestRoom", 10, 10);

            var heater = room.AddHeater(1, 2, 21.5f);
            room.RemoveHeater(heater.Id);

            Assert.AreEqual(0, room.Heaters.Count);
        }

        [TestMethod]
        public void RoomData_AddHeatSensor_IncreasesSensorCount()
        {
            var room = new DummyRoomData(Guid.NewGuid(), "TestRoom", 10, 10);

            var sensor = room.AddHeatSensor(5, 5);

            Assert.AreEqual(1, room.HeatSensors.Count);
            Assert.IsTrue(room.ContainsHeatSensor(sensor.Id));
        }

        [TestMethod]
        public void RoomData_ClearHeaters_RemovesAll()
        {
            var room = new DummyRoomData(Guid.NewGuid(), "Room1", 10, 10);

            room.AddHeater(1, 1, 20);
            room.AddHeater(2, 2, 21);

            room.ClearHeaters();

            Assert.AreEqual(0, room.Heaters.Count);
        }

        [TestMethod]
        public void RoomData_TemperatureChangedEvent_HeaterTriggersRoomEvent()
        {
            var room = new DummyRoomData(Guid.NewGuid(), "TestRoom", 10, 10);
            bool eventTriggered = false;

            room.TemperatureChanged += (s, oldT, newT) => eventTriggered = true;

            var heater = room.AddHeater(1, 1, 25.0f);
            heater.Temperature = 26.0f;

            Assert.IsTrue(eventTriggered);
        }

        [TestMethod]
        public void RoomData_EnableChangedEvent_HeaterTriggersRoomEvent()
        {
            var room = new DummyRoomData(Guid.NewGuid(), "TestRoom", 10, 10);
            bool eventTriggered = false;

            room.EnableChanged += (s, oldE, newE) => eventTriggered = true;

            var heater = room.AddHeater(1, 1, 20.0f);
            heater.IsOn = true;

            Assert.IsTrue(eventTriggered);
        }

        [TestMethod]
        public void RoomData_PositionChangedEvent_HeatSensorTriggersRoomEvent()
        {
            var room = new DummyRoomData(Guid.NewGuid(), "TestRoom", 10, 10);
            bool eventTriggered = false;

            room.PositionChanged += (s, oldP, newP) => eventTriggered = true;

            var sensor = room.AddHeatSensor(2, 2);
            ((DummyPositionData)sensor.Position).X = 3;

            Assert.IsTrue(eventTriggered);
        }
    }
}