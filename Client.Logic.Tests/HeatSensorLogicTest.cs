using TPUM.Client.Data;

namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class HeatSensorLogicTest
    {
        private DataApiBase _dataApi = default!;
        private IHeatSensorLogic _sensor = default!;
        private const float _x = 2f;
        private const float _y = 2f;

        [TestInitialize]
        public void Setup()
        {
            _dataApi = DataApiBase.GetApi();
            _sensor = new DummyHeatSensorLogic(_dataApi.CreateHeatSensor(_x, _y));
        }

        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            Assert.AreEqual(_x, _sensor.Position.X);
            Assert.AreEqual(_y, _sensor.Position.Y);
            Assert.AreEqual(0.0f, _sensor.Temperature, 1e-10f);
        }

        [TestMethod]
        public void Temperature_SetNewValue_ShouldTriggerEvent()
        {
            bool eventTriggered = false;
            _sensor.TemperatureChanged += (sender, args) =>
            {
                eventTriggered = true;
                Assert.AreEqual(0f, args.LastTemperature, 1e-10f);
                Assert.AreEqual(100.0f, args.NewTemperature, 1e-10f);
            };

            ((DummyHeatSensorLogic)_sensor).SetTemperature(100.0f);

            Assert.IsTrue(eventTriggered, "TemperatureChange event was not triggered.");
        }

        [TestMethod]
        public void Temperature_SetSameValue_ShouldNotTriggerEvent()
        {
            bool eventTriggered = false;
            _sensor.TemperatureChanged += (s, e) => eventTriggered = true;

            ((DummyHeatSensorLogic)_sensor).SetTemperature(0.0f);

            Assert.IsFalse(eventTriggered);
        }

        [TestMethod]
        public void Position_SetNewValue_ShouldTriggerEvent()
        {
            bool eventTriggered = false;
            _sensor.PositionChanged += (sender, args) =>
            {
                eventTriggered = true;
                Assert.AreEqual(_x, args.LastPosition.X, 1e-10f);
                Assert.AreEqual(_y, args.LastPosition.Y, 1e-10f);
                Assert.AreEqual(5f, args.NewPosition.X, 1e-10f);
                Assert.AreEqual(5f, args.NewPosition.Y, 1e-10f);
            };

            _sensor.Position = new DummyPositionLogic(_dataApi.CreatePosition(5f, 5f));

            Assert.IsTrue(eventTriggered, "PositionChange event was not triggered.");
        }

        [TestMethod]
        public void Position_SetSameValue_ShouldNotTriggerEvent()
        {
            bool eventTriggered = false;
            _sensor.PositionChanged += (s, e) => eventTriggered = true;

            _sensor.Position = new DummyPositionLogic(_dataApi.CreatePosition(_x, _y));

            Assert.IsFalse(eventTriggered);
        }

        [TestMethod]
        public void HashCode_ShouldReturnConsistentValue()
        {
            var hash = 3 * _sensor.Position.GetHashCode() + 5 * _sensor.Temperature.GetHashCode();
            Assert.AreEqual(hash, _sensor.GetHashCode(), 1);
        }
    }
}