namespace TPUM.Client.Presentation.Model.Tests
{
    [TestClass]
    public sealed class HeatSensorTest
    {
        [TestMethod]
        public void HeatSensor_GetTemperature_ShouldReturnProperTemperature()
        {
            var sensor = new DummyHeatSensorModel(new TestHeatSensorLogic(0f, 0f, 19f));
            Assert.AreEqual(19f, sensor.Temperature, 1e-10f);
        }
    }
}
