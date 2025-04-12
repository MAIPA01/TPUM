namespace TPUM.Client.Presentation.Model.Tests
{
    [TestClass]
    public sealed class RoomTest
    {
        [TestMethod]
        public void Room_ShouldAddHeaterAndSensorCorrectly()
        {
            var logic = new TestRoomLogic();
            var room = new DummyRoomModel(logic);

            room.AddHeater(0f, 0f, 0f);
            room.AddHeatSensor(0f, 0f);

            Assert.AreEqual(2, room.Heaters.Count);
            Assert.AreEqual(2, room.HeatSensors.Count);

            var heater = room.Heaters.Last();
            var sensor = room.HeatSensors.Last();

            Assert.IsNotNull(heater);
            Assert.IsNotNull(sensor);
        }

        [TestMethod]
        public void Room_RemoveHeater_ShouldWorkCorrectly()
        {
            var logic = new TestRoomLogic();
            var room = new DummyRoomModel(logic);

            room.AddHeater(0f, 0f, 0f);
            Assert.AreEqual(2, room.Heaters.Count);

            var heater = room.Heaters.Last();

            Assert.IsNotNull(heater);

            room.RemoveHeater(heater.Id);

            Assert.AreEqual(1, room.Heaters.Count);
        }

        [TestMethod]
        public void Room_RemoveHeatSensor_ShouldWorkCorrectly()
        {
            var logic = new TestRoomLogic();
            var room = new DummyRoomModel(logic);

            room.AddHeatSensor(0f, 0f);
            Assert.AreEqual(2, room.HeatSensors.Count);

            var sensor = room.HeatSensors.Last();

            Assert.IsNotNull(sensor);

            room.RemoveHeatSensor(sensor.Id);

            Assert.AreEqual(1, room.HeatSensors.Count);
        }
    }
}
