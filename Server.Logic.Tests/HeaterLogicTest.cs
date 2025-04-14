namespace TPUM.Server.Logic.Tests
{
    [TestClass]
    public sealed class HeaterLogicTest
    {
        [TestMethod]
        public void HeaterLogic_TurnOn_SetsIsOnTrue()
        {
            var data = new HeaterData();
            var logic = new DummyHeaterLogic(data);

            logic.TurnOn();

            Assert.IsTrue(logic.IsOn);
        }

        [TestMethod]
        public void HeaterLogic_TurnOff_SetsIsOnFalse()
        {
            var data = new HeaterData();
            var logic = new DummyHeaterLogic(data);

            logic.TurnOn();
            logic.TurnOff();

            Assert.IsFalse(logic.IsOn);
        }

        [TestMethod]
        public void HeaterLogic_SetTemperature_ChangesTemperature()
        {
            var data = new HeaterData();
            var logic = new DummyHeaterLogic(data);

            logic.Temperature = 42.5f;

            Assert.AreEqual(42.5f, logic.Temperature);
        }
    }
}