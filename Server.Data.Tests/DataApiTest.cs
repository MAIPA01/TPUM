namespace TPUM.Server.Data.Tests
{
    [TestClass]
    public sealed class DataApiTest
    {
        private DataApiBase _dataApi = default!;
        private const float _width = 10f;
        private const float _height = 20f;

        [TestInitialize]
        public void Setup()
        {
            _dataApi = DataApiBase.GetApi();
        }

        [TestMethod]
        public void CreatePosition_ShouldReturnValidPosition()
        {
            IPositionData position = _dataApi.CreatePosition(10f, 20f);

            Assert.IsNotNull(position);
            Assert.AreEqual(10f, position.X);
            Assert.AreEqual(20f, position.Y);
        }

        [TestMethod]
        public void CreateHeater_ShouldReturnValidPosition()
        {
            IHeaterData heater = _dataApi.CreateHeater(10f, 20f, 100f);

            Assert.IsNotNull(heater);
            Assert.AreEqual(10f, heater.Position.X);
            Assert.AreEqual(20f, heater.Position.Y);
            Assert.AreEqual(100f, heater.Temperature);
        }

        [TestMethod]
        public void CreateHeatSensor_ShouldReturnValidPosition()
        {
            IHeatSensorData sensor = _dataApi.CreateHeatSensor(10f, 20f);

            Assert.IsNotNull(sensor);
            Assert.AreEqual(10f, sensor.Position.X);
            Assert.AreEqual(20f, sensor.Position.Y);
            Assert.AreEqual(0f, sensor.Temperature);
        }

        [TestMethod]
        public void AddRoom_ShouldAddRoomToList()
        {
            IRoomData room = _dataApi.AddRoom(_width, _height);
            var rooms = _dataApi.Rooms;

            Assert.AreEqual(1, rooms.Count);
            Assert.AreSame(room, rooms.First());
            Assert.IsTrue(rooms.Contains(room));
            Assert.AreEqual(_width, room.Width);
            Assert.AreEqual(_height, room.Height);
        }

        [TestMethod]
        public void RemoveRoom_ShouldRemoveRoomFromList()
        {
            IRoomData room = _dataApi.AddRoom(_width, _height);

            Assert.AreEqual(1, _dataApi.Rooms.Count);

            _dataApi.RemoveRoom(room.Id);

            Assert.AreEqual(0, _dataApi.Rooms.Count);
        }

        [TestMethod]
        public void ClearRooms_ShouldRemoveAllRooms()
        {
            _dataApi.AddRoom(_width, _height);
            _dataApi.AddRoom(_width + 20f, _height + 20f);

            Assert.AreEqual(2, _dataApi.Rooms.Count);

            _dataApi.ClearRooms();

            Assert.AreEqual(0, _dataApi.Rooms.Count);
        }

        [TestMethod]
        public void Dispose_ShouldClearRoomsAndDisposeEachRoom()
        {
            _dataApi.AddRoom(_width, _height);
            _dataApi.AddRoom(_width + 20f, _height + 20f);

            Assert.AreEqual(2, _dataApi.Rooms.Count);

            _dataApi.Dispose();

            Assert.AreEqual(0, _dataApi.Rooms.Count);
        }

        [TestMethod]
        public void Dispose_ShouldNotThrowException()
        {
            _dataApi.Dispose();
        }
    }
}