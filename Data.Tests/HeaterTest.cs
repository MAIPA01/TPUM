namespace TPUM.Data.Tests
{
    [TestClass]
    public sealed class HeaterTest
    {
        private Heater _heater;
        private const long _id = 10;
        private const float _x = 2f;
        private const float _y = 2f;
        private const float _temp = 21f;

        [TestInitialize]
        public void Setup()
        {
            _heater = new(_id, _x, _y, _temp);
        }

        [TestMethod]
        public void Heater_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(_id, _heater.Id);
            Assert.AreEqual(new Position(_x, _y), _heater.Position);
            Assert.AreEqual(0f, _heater.Temperature, 1e-10f);
            Assert.IsFalse(_heater.IsOn);
        }

        [TestMethod]
        public void SetPosition_ShouldUpdatePositionCorrectly()
        {
            Assert.AreEqual(new Position(_x, _y), _heater.Position);
            _heater.Position.X += 1f;
            Assert.AreEqual(new Position(_x + 1f, _y), _heater.Position);
        }

        [TestMethod]
        public void IsOn_ShouldReturnCorrectState_WhenTurnedOnAndOff()
        {
            Assert.IsFalse(_heater.IsOn);

            _heater.TurnOn();
            Assert.IsTrue(_heater.IsOn);

            _heater.TurnOff();
            Assert.IsFalse(_heater.IsOn);
        }

        [TestMethod]
        public void IsOn_ShouldNotChangeIfTurnedOnTwice()
        {
            Assert.IsFalse(_heater.IsOn);

            _heater.TurnOn();
            Assert.IsTrue(_heater.IsOn);

            _heater.TurnOn();
            Assert.IsTrue(_heater.IsOn);
        }

        [TestMethod]
        public void Temperature_ShouldReturnZero_WhenHeaterIsOff()
        {
            _heater.TurnOff();
            Assert.AreEqual(0f, _heater.Temperature, 1e-10f);
        }

        [TestMethod]
        public void Temperature_ShouldReturnCorrectValue_WhenHeaterIsOn()
        {
            _heater.TurnOff();
            Assert.AreEqual(0f, _heater.Temperature, 1e-10f);

            _heater.TurnOn();
            Assert.AreEqual(_temp, _heater.Temperature, 1e-10f);
        }

        [TestMethod]
        public void SetTemperature_ShouldUpdateTemperatureCorrectly()
        {
            _heater.TurnOn();
            _heater.Temperature = _temp + 1f;
            Assert.AreEqual(_temp + 1f, _heater.Temperature, 1e-10f);
        }

        [TestMethod]
        public void TemperatureChangedEvent_ShouldBeTriggered_WhenTemperatureChanges()
        {
            bool eventTriggered = false;
            _heater.TemperatureChanged += (sender, args) =>
            {
                eventTriggered = true;
                Assert.AreEqual(_temp, args.LastTemperature, 1e-10f);
                Assert.AreEqual(30.0f, args.NewTemperature, 1e-10f);
            };

            _heater.Temperature = 30.0f;

            Assert.IsTrue(eventTriggered, "TemperatureChanged event was not triggered.");
        }

        [TestMethod]
        public void EnableChangedEvent_ShouldBeTriggered_WhenHeaterIsTurnedOn()
        {
            bool eventTriggered = false;
            _heater.EnableChanged += (sender, args) =>
            {
                eventTriggered = true;
                Assert.IsFalse(args.LastEnable);
                Assert.IsTrue(args.NewEnable);
            };

            _heater.TurnOn();

            Assert.IsTrue(eventTriggered, "EnableChange event was not triggered.");
        }

        [TestMethod]
        public void EnableChangedEvent_ShouldBeTriggered_WhenHeaterIsTurnedOff()
        {
            _heater.TurnOn();
            bool eventTriggered = false;
            _heater.EnableChanged += (sender, args) =>
            {
                eventTriggered = true;
                Assert.IsTrue(args.LastEnable);
                Assert.IsFalse(args.NewEnable);
            };

            _heater.TurnOff();

            Assert.IsTrue(eventTriggered, "EnableChange event was not triggered.");
        }
    }
}