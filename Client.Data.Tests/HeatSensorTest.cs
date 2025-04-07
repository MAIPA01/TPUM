namespace TPUM.Client.Data.Tests
{
    [TestClass]
    public sealed class HeatSensorTest
    {
        private IHeatSensor _sensor = default!;
        private const long _id = 10;
        private const float _x = 2f;
        private const float _y = 2f;

        [TestInitialize]
        public void Setup()
        {
            _sensor = new DummyHeatSensor(_id, _x, _y);
        }

        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            Assert.AreEqual(_id, _sensor.Id);
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

            _sensor.Temperature = 100.0f;

            Assert.IsTrue(eventTriggered, "TemperatureChange event was not triggered.");
        }

        [TestMethod]
        public void Temperature_SetSameValue_ShouldNotTriggerEvent()
        {
            bool eventTriggered = false;
            _sensor.TemperatureChanged += (s, e) => eventTriggered = true;

            _sensor.Temperature = 0.0f;

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

            _sensor.Position = new Position(5.0f, 5.0f);

            Assert.IsTrue(eventTriggered, "TemperatureChange event was not triggered.");
        }

        [TestMethod]
        public void Position_SetSameValue_ShouldNotTriggerEvent()
        {
            bool eventTriggered = false;
            _sensor.PositionChanged += (s, e) => eventTriggered = true;

            _sensor.Position = new Position(_x, _y);

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