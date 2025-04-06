using TPUM.Data;

namespace TPUM.Logic.Tests
{
    [TestClass]
    public sealed class RoomSimulationTest
    {
        private DataApiBase _dataApi = default!;
        private IRoom _room = default!;

        [TestInitialize]
        public void Setup()
        {
            _dataApi = DataApiBase.GetApi();
            _room = new DummyRoom(_dataApi.AddRoom(10f, 20f));
        }

        [TestMethod]
        public void UpdateTemperature_ShouldUpdateSensorTemperaturesBasedOnHeater()
        {
            IHeater heater = _room.AddHeater(5f, 5f, 100f);
            heater.TurnOn();

            IHeatSensor sensor1 = _room.AddHeatSensor(3f, 3f);
            ((DummyHeatSensor)sensor1).SetTemperature(20f);
            IHeatSensor sensor2 = _room.AddHeatSensor(7f, 7f);
            ((DummyHeatSensor)sensor2).SetTemperature(20f);

            _room.StartSimulation();

            Thread.Sleep(20);
            _room.EndSimulation();

            Assert.IsTrue(sensor1.Temperature > 20f, $"Expected sensor1 temperature to be greater than 20, but got {sensor1.Temperature}");
            Assert.IsTrue(sensor2.Temperature > 20f, $"Expected sensor2 temperature to be greater than 20, but got {sensor2.Temperature}");
        }
    };
}
