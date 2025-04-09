using TPUM.Client.Logic;

namespace TPUM.Client.Presentation.Model.Tests
{
    [TestClass]
    public sealed class HeaterTest
    {
        private ModelApiBase _modelApi = default!;

        [TestInitialize]
        public void Setup()
        {
            _modelApi = ModelApiBase.GetApi("ws://localhost:5000/ws");
        }


        [TestMethod]
        public void Heater_TemperatureChange_ShouldTriggerEvent()
        {
            var logic = new TestHeaterLogic(20f);
            var heater = new DummyHeater(logic, _modelApi);

            bool eventTriggered = false;
            heater.TemperatureChanged += (s, e) => eventTriggered = true;

            heater.Temperature = 25f;

            Assert.IsTrue(eventTriggered);
        }

        [TestMethod]
        public void Heater_PositionChange_ShouldTriggerEvent()
        {
            var logic = new TestHeaterLogic();
            var heater = new DummyHeater(logic, _modelApi);

            bool eventTriggered = false;
            heater.PositionChanged += (s, e) => eventTriggered = true;

            heater.Position = new DummyPosition(new TestPositionLogic(3, 4));

            Assert.IsTrue(eventTriggered);
        }
    }
}
