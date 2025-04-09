namespace TPUM.Client.Presentation.Model.Tests
{
    [TestClass]
    public sealed class HeatSensorTest
    {
        [TestMethod]
        public void HeatSensor_UpdateTemperature_ShouldTriggerEvent()
        {
            var logic = new TestHeatSensorLogic(19f);
            var sensor = new DummyHeatSensor(logic);

            bool eventTriggered = false;
            sensor.TemperatureChanged += (s, e) => eventTriggered = true;

            sensor.UpdateData(1, 1, 22f);

            Assert.IsTrue(eventTriggered);
        }
    }
}
