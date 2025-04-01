namespace TPUM.Data.Tests
{
    [TestClass]
    public sealed class DataApiTest
    {
        [TestMethod]
        public void TestCreateHeatSensor()
        {
            var sensor = DataApiBase.GetApi().CreateHeatSensor(2f, 2f);
            Assert.AreEqual(2f, sensor.Position.X, 1e-10f);
            Assert.AreEqual(2f, sensor.Position.Y, 1e-10f);
            Assert.AreEqual(0f, sensor.Temperature, 1e-10f);
        }

        [TestMethod]
        public void TestCreateHeater()
        {
            var heater = DataApiBase.GetApi().CreateHeater(2f, 2f, 21f);
            Assert.AreEqual(2f, heater.Position.X, 1e-10f);
            Assert.AreEqual(2f, heater.Position.Y, 1e-10f);
            heater.TurnOn();
            Assert.AreEqual(21f, heater.Temperature, 1e-10f);
        }
    }
}
