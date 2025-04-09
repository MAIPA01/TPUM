namespace TPUM.Server.Logic.Tests
{
    [TestClass]
    public sealed class HeatSensorLogicTest
    {
        [TestMethod]
        public void HeatSensorLogic_Temperature_ReturnsSetValue()
        {
            var data = new HeatSensorData();
            var logic = new DummyHeatSensorLogic(data);

            logic.SetTemperature(21.5f);

            Assert.AreEqual(21.5f, logic.Temperature);
        }
    }
}
