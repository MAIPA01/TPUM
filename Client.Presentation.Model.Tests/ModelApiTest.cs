namespace TPUM.Client.Presentation.Model.Tests
{
    [TestClass]
    public sealed class ModelApiTest
    {
        private ModelApiBase _modelApi = default!;
        private const string _roomName = "Test";
        private const float _roomWidth = 5.0f;
        private const float _roomHeight = 10.0f;

        [TestInitialize]
        public void Setup()
        {
            _modelApi = ModelApiBase.GetApi();
        }

        [TestMethod]
        public void ModelApi_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(0, _modelApi.Rooms.Count);
        }

        [TestMethod]
        public void AddRoom_ShouldAddRoomToRoomsList()
        {
            var room = _modelApi.AddRoom(_roomName, _roomWidth, _roomHeight);

            Assert.IsNotNull(room);
            Assert.AreEqual(1, _modelApi.Rooms.Count);
            Assert.AreSame(room, _modelApi.Rooms.First());
        }

        [TestMethod]
        public void AddRoom_ShouldAddRoomToRoomsListWithCorrectValues()
        {
            var room = _modelApi.AddRoom(_roomName, _roomWidth, _roomHeight);

            Assert.IsNotNull(room);
            Assert.AreEqual(1, _modelApi.Rooms.Count);
            Assert.AreSame(room, _modelApi.Rooms.First());
            Assert.AreEqual(_roomName, room.Name);
            Assert.AreEqual(_roomWidth, room.Width);
            Assert.AreEqual(_roomHeight, room.Height);
        }

        [TestMethod]
        public void RemoveRoom_ShouldRemoveRoomFromRoomsList()
        {
            var room = _modelApi.AddRoom(_roomName, _roomWidth, _roomHeight);
            Assert.AreEqual(1, _modelApi.Rooms.Count);

            _modelApi.RemoveRoom(room.Id);
            Assert.AreEqual(0, _modelApi.Rooms.Count);
        }

        [TestMethod]
        public void RemoveRoom_ShouldNotRemoveRoomIfIdNotFound()
        {
            _modelApi.AddRoom(_roomName, _roomWidth, _roomHeight);
            Assert.AreEqual(1, _modelApi.Rooms.Count);

            _modelApi.RemoveRoom(1);
            Assert.AreEqual(1, _modelApi.Rooms.Count);
        }

        [TestMethod]
        public void Dispose_ShouldClearRoomsAfterDisposing()
        {
            var room = _modelApi.AddRoom(_roomName, _roomWidth, _roomHeight);
            Assert.AreEqual(1, _modelApi.Rooms.Count);

            _modelApi.Dispose();
            Assert.AreEqual(0, _modelApi.Rooms.Count);
        }
    }
}
