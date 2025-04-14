namespace TPUM.Client.Presentation.Model.Tests
{
    [TestClass]
    public sealed class HeaterTest
    {

        [TestMethod]
        public void Heater_GetTemperature_ShouldReturnProperTemperature()
        {
            var logic = new TestHeaterLogic(20f);
            var heater = new DummyHeater(logic);
            Assert.AreEqual(20f, heater.Temperature, 1e-10f);
        }

        [TestMethod]
        public void Heater_GetPosition_ShouldReturnProperPosition()
        {
            var logic = new TestHeaterLogic(0f, 0f);
            var heater = new DummyHeater(logic);
            Assert.AreEqual(0f, heater.Position.X, 1e-10f);
            Assert.AreEqual(0f, heater.Position.Y, 1e-10f);
        }
    }
}