namespace TPUM.Server.Logic.Tests
{
    [TestClass]
    public sealed class RoomLogicTest
    {
        [TestMethod]
        public void RoomLogic_AddHeater_StoresHeater()
        {
            var data = new RoomData();
            var room = new DummyRoomLogic(data);

            var heater = room.AddHeater(1f, 1f, 25f);

            Assert.IsTrue(room.Heaters.Contains(heater));
        }

        [TestMethod]
        public void RoomLogic_AddHeatSensor_StoresSensor()
        {
            var data = new RoomData();
            var room = new DummyRoomLogic(data);

            var sensor = room.AddHeatSensor(2f, 2f);

            Assert.IsTrue(room.HeatSensors.Contains(sensor));
        }

        [TestMethod]
        public void RoomLogic_ContainsHeater_ReturnsTrueWhenExists()
        {
            var data = new RoomData();
            var room = new DummyRoomLogic(data);

            var heater = room.AddHeater(1f, 1f, 30f);

            Assert.IsTrue(room.ContainsHeater(heater.Id));
        }

        [TestMethod]
        public void RoomLogic_GetHeater_ReturnsCorrectHeater()
        {
            var data = new RoomData();
            var room = new DummyRoomLogic(data);

            var heater = room.AddHeater(1f, 1f, 30f);

            var found = room.GetHeater(heater.Id);
            Assert.AreEqual(heater, found);
        }
    }
}
