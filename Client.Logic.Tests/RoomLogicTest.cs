namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class RoomLogicTest
    {
        private IRoomLogic? _room = null;

        [TestInitialize]
        public void Setup()
        {
            _room = new DummyRoomLogic(new TestRoomData());
        }

        [TestMethod]
        public void Room_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(5f, _room!.Width, 1e-10f);
            Assert.AreEqual(4f, _room!.Height, 1e-10f);
            Assert.AreEqual(0, _room!.Heaters.Count);
            Assert.AreEqual(0, _room!.HeatSensors.Count);
        }

        [TestMethod]
        public void RoomLogic_CanAddAndRemoveHeater()
        {
            var room = new DummyRoomLogic(new TestRoomData());

            room.AddHeater(0f, 0f, 0f);
            Assert.AreEqual(1, room.Heaters.Count);

            var heater = room.Heaters.Last();

            Assert.IsTrue(room.Heaters.Contains(heater));

            room.RemoveHeater(heater.Id);

            Assert.IsFalse(room.Heaters.Contains(heater));
            Assert.AreEqual(0, room.Heaters.Count);
        }

        [TestMethod]
        public void RoomLogic_CanAddAndRemoveHeatSensor()
        {
            var room = new DummyRoomLogic(new TestRoomData());

            room.AddHeatSensor(0f, 0f);
            Assert.AreEqual(1, room.HeatSensors.Count);

            var sensor = room.HeatSensors.Last();

            Assert.IsTrue(room.HeatSensors.Contains(sensor));

            room.RemoveHeatSensor(sensor.Id);

            Assert.IsFalse(room.HeatSensors.Contains(sensor));
            Assert.AreEqual(0, room.HeatSensors.Count);
        }
    };
}