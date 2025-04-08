namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class LogicAPITest
    {
        private LogicApiBase _logicApi = default!;

        [TestInitialize]
        public void Setup()
        {
            _logicApi = LogicApiBase.GetApi();
        }

        [TestMethod]
        public void LogicApi_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(0, _logicApi.Rooms.Count);
        }

        [TestMethod]
        public void CreatePosition_ShouldReturnValidPosition()
        {
            IPositionLogic position = _logicApi.CreatePosition(10f, 20f);

            Assert.IsNotNull(position);
            Assert.AreEqual(10f, position.X);
            Assert.AreEqual(20f, position.Y);
        }

        [TestMethod]
        public void CreateHeater_ShouldReturnValidPosition()
        {
            IHeaterLogic heater = _logicApi.CreateHeater(10f, 20f, 100f);

            Assert.IsNotNull(heater);
            Assert.AreEqual(10f, heater.Position.X);
            Assert.AreEqual(20f, heater.Position.Y);
            Assert.AreEqual(0f, heater.Temperature);
            heater.TurnOn();
            Assert.AreEqual(100f, heater.Temperature);
        }

        [TestMethod]
        public void CreateHeatSensor_ShouldReturnValidPosition()
        {
            IHeatSensorLogic sensor = _logicApi.CreateHeatSensor(10f, 20f);

            Assert.IsNotNull(sensor);
            Assert.AreEqual(10f, sensor.Position.X);
            Assert.AreEqual(20f, sensor.Position.Y);
            Assert.AreEqual(0f, sensor.Temperature);
        }

        [TestMethod]
        public void AddRoom_ShouldAddRoomToRoomsList()
        {
            var room = _logicApi.AddRoom(5.0f, 10.0f);

            Assert.IsNotNull(room);
            Assert.AreEqual(1, _logicApi.Rooms.Count);
            Assert.AreSame(room, _logicApi.Rooms.First());
        }

        [TestMethod]
        public void RemoveRoom_ShouldRemoveRoomFromRoomsList()
        {
            var room = _logicApi.AddRoom(5.0f, 10.0f);
            Assert.AreEqual(1, _logicApi.Rooms.Count);

            _logicApi.RemoveRoom(room.Id);
            Assert.AreEqual(0, _logicApi.Rooms.Count);
        }

        [TestMethod]
        public void RemoveRoom_ShouldNotRemoveRoomIfIdNotFound()
        {
            _logicApi.AddRoom(5.0f, 10.0f);
            Assert.AreEqual(1, _logicApi.Rooms.Count);

            _logicApi.RemoveRoom(Guid.Empty);
            Assert.AreEqual(1, _logicApi.Rooms.Count);
        }

        [TestMethod]
        public void Dispose_ShouldClearRoomsAfterDisposing()
        {
            var room = _logicApi.AddRoom(5.0f, 10.0f);
            Assert.AreEqual(1, _logicApi.Rooms.Count);

            _logicApi.Dispose();
            Assert.AreEqual(0, _logicApi.Rooms.Count);
        }
    }
}