namespace TPUM.Data.Tests
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
        public void AddRoom_ShouldAddRoomToList()
        {
            IRoom room = _dataApi.AddRoom(_width, _height);
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
            IRoom room = _dataApi.AddRoom(_width, _height);

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