namespace TPUM.Client.Data.Tests
{
    [TestClass]
    public sealed class RoomDataTest
    {
        private IRoomData? _room = null;
        private static readonly Guid Id = Guid.Parse("44465E20-25A1-4FA3-BE1D-0AB7E5C739F4");
        private static readonly Guid Test = Guid.Parse("44465E20-25A1-4FA3-BE1D-0AB7E5C73901");
        private const string Name = "name";
        private const float Width = 100f;
        private const float Height = 100f;

        [TestInitialize]
        public void Setup()
        {
            _room = new DummyRoomData(Id, Name, Width, Height);
        }

        [TestMethod]
        public void Room_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(Id, _room!.Id);
            Assert.AreEqual(Name, _room!.Name);
            Assert.AreEqual(Width, _room!.Width, 1e-10f);
            Assert.AreEqual(Height, _room!.Height, 1e-10f);
            Assert.AreEqual(0, _room!.Heaters.Count);
            Assert.AreEqual(0, _room!.HeatSensors.Count);
        }

        [TestMethod]
        public void AddHeater_ShouldAddHeaterToRoom()
        {
            _room!.AddHeater(5f, 5f, 100f);
            Assert.AreEqual(1, _room.Heaters.Count);

            var heater = _room!.Heaters.First();
            Assert.IsNotNull(heater);
            Assert.AreEqual(100f, heater.Temperature);
            Assert.AreEqual(5f, heater.Position.X);
            Assert.AreEqual(5f, heater.Position.Y);
            Assert.AreEqual(false, heater.IsOn);
        }

        [TestMethod]
        public void AddHeater_ShouldNotThrowException_WhenPositionOutOfRange()
        {
            _room!.AddHeater(110f, 5f, 100f);
        }

        [TestMethod]
        public void RemoveHeater_ShouldNotRemoveHeaterFromRoomIfIdNotFound()
        {
            _room!.AddHeater(5f, 5f, 100f);

            _room!.RemoveHeater(Guid.Empty);

            Assert.AreEqual(1, _room!.Heaters.Count);
        }

        [TestMethod]
        public void RemoveHeater_ShouldRemoveHeaterFromRoom()
        {
            _room!.AddHeater(5f, 5f, 100f);

            _room!.RemoveHeater(_room!.Heaters.First().Id);

            Assert.AreEqual(0, _room!.Heaters.Count);
        }

        [TestMethod]
        public void AddHeatSensor_ShouldAddSensorToRoom()
        {
            _room!.AddHeatSensor(5f, 5f);
            Assert.AreEqual(1, _room.HeatSensors.Count);

            var sensor = _room!.HeatSensors.First();

            Assert.IsNotNull(sensor);
            Assert.AreEqual(0f, sensor.Temperature);
            Assert.AreEqual(5f, sensor.Position.X);
            Assert.AreEqual(5f, sensor.Position.Y);
        }

        [TestMethod]
        public void AddHeatSensor_ShouldNotThrowException_WhenPositionOutOfRange()
        {
            _room!.AddHeatSensor(110f, 5f);
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldNotRemoveHeatSensorFromRoomIfIdNotFound()
        {
            _room!.AddHeatSensor(5f, 5f);

            _room!.RemoveHeatSensor(Guid.Empty);

            Assert.AreEqual(1, _room!.HeatSensors.Count);
        }

        [TestMethod]
        public void RemoveHeatSensor_ShouldRemoveHeatSensorFromRoom()
        {
            _room!.AddHeatSensor(5f, 5f);

            _room!.RemoveHeatSensor(_room.HeatSensors.First().Id);

            Assert.AreEqual(0, _room!.HeatSensors.Count);
        }
    }
}