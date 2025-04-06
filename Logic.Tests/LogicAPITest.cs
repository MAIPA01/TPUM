namespace TPUM.Logic.Tests
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

            _logicApi.RemoveRoom(1);
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