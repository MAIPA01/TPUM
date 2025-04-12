namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class HeaterLogicTest
    {
        private IHeaterLogic? _heater = null;

        [TestInitialize]
        public void Setup()
        {
            _heater = new DummyHeaterLogic(new TestHeaterData
            {
                Temperature = 22.5f,
                IsOn = false
            });
        }

        [TestMethod]
        public void Heater_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(0f, _heater!.Position.X);
            Assert.AreEqual(0f, _heater!.Position.Y);
            Assert.IsFalse(_heater!.IsOn);
            Assert.AreEqual(22.5f, _heater!.Temperature, 1e-10f);
        }

        [TestMethod]
        public void SetPosition_ShouldUpdatePositionCorrectly()
        {
            Assert.AreEqual(0f, _heater!.Position.X);
            Assert.AreEqual(0f, _heater!.Position.Y);
            _heater!.SetPosition(1f, 1f);
            Assert.AreEqual(1f, _heater!.Position.X);
            Assert.AreEqual(1f, _heater!.Position.Y);
        }

        [TestMethod]
        public void HeaterLogic_ShouldReturnTemperature_IfOn()
        {
            var data = new TestHeaterData
            {
                Temperature = 22.5f,
                IsOn = true
            };
            var logic = new DummyHeaterLogic(data);

            Assert.AreEqual(22.5f, logic.Temperature);
        }

        [TestMethod]
        public void HeaterLogic_ShouldReturnTemperature_IfOff()
        {
            var data = new TestHeaterData
            {
                Temperature = 22.5f,
                IsOn = false
            };
            var logic = new DummyHeaterLogic(data);

            Assert.AreEqual(22.5f, logic.Temperature);
        }
    }
}