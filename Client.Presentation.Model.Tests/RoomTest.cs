namespace TPUM.Client.Presentation.Model.Tests
{
    [TestClass]
    public sealed class RoomTest
    {
        private ModelApiBase _modelApi = default!;

        [TestInitialize]
        public void Setup()
        {
            _modelApi = ModelApiBase.GetApi("ws://localhost:5000/ws");
        }

        [TestMethod]
        public void Room_ShouldAddHeaterAndSensorCorrectly()
        {
            var logic = new TestRoomLogic();
            var room = new DummyRoom(logic, _modelApi);

            var heater = room.AddHeater(new DummyHeater(new TestHeaterLogic(), _modelApi));
            var sensor = room.AddHeatSensor(new DummyHeatSensor(new TestHeatSensorLogic()));

            Assert.IsNotNull(heater);
            Assert.IsNotNull(sensor);
            Assert.AreEqual(2, room.Heaters.Count);
            Assert.AreEqual(2, room.HeatSensors.Count);
        }

        [TestMethod]
        public void Room_RemoveHeater_ShouldWorkCorrectly()
        {
            var logic = new TestRoomLogic();
            var room = new DummyRoom(logic, _modelApi);

            var heater = room.AddHeater(new DummyHeater(new TestHeaterLogic(), _modelApi));
            Assert.IsNotNull(heater);

            room.RemoveHeater(heater.Id);

            Assert.AreEqual(1, room.Heaters.Count);
        }

        [TestMethod]
        public void Room_RemoveHeatSensor_ShouldWorkCorrectly()
        {
            var logic = new TestRoomLogic();
            var room = new DummyRoom(logic, _modelApi);

            var sensor = room.AddHeatSensor(new DummyHeatSensor(new TestHeatSensorLogic()));
            Assert.IsNotNull(sensor);

            room.RemoveHeatSensor(sensor.Id);

            Assert.AreEqual(1, room.HeatSensors.Count);
        }
    }
}
