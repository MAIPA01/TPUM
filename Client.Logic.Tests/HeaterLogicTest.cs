namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class HeaterLogicTest
    {
        private IHeaterLogic _heater = default!;
        private IPositionLogic _position = default!;

        public class TestPositionData : Data.IPositionData
        {
            public float X { get; set; }
            public float Y { get; set; }
        }

        public class TestHeaterData : Data.IHeaterData
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Data.IPositionData Position { get; set; } = new TestPositionData();
            private float _temperature = 0.0f;
            public float Temperature { get; set; }
            public bool IsOn { get; set; }
        }

        [TestInitialize]
        public void Setup()
        {
            var _posData = new TestPositionData { X = 1.0f, Y = 2.0f };
            _position = new DummyPositionLogic(_posData);
            _heater = new DummyHeaterLogic(new TestHeaterData
            {
                Position = _posData,
                Temperature = 22.5f,
                IsOn = false
            });
        }

        [TestMethod]
        public void Heater_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(_position.X, _heater.Position.X);
            Assert.AreEqual(_position.Y, _heater.Position.Y);
            Assert.IsFalse(_heater.IsOn);
            Assert.AreEqual(0f, _heater.Temperature, 1e-10f);
        }

        [TestMethod]
        public void SetPosition_ShouldUpdatePositionXCorrectly()
        {
            Assert.AreEqual(_position.X, _heater.Position.X);
            _heater.Position.X += 1f;
            _position.X += 1f;
            Assert.AreEqual(_position.X, _heater.Position.X);
        }

        [TestMethod]
        public void SetPosition_ShouldUpdatePositionYCorrectly()
        {
            Assert.AreEqual(_position.Y, _heater.Position.Y);
            _heater.Position.Y += 1f;
            _position.Y += 1f;
            Assert.AreEqual(_position.Y, _heater.Position.Y);
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
        public void HeaterLogic_ShouldReturnZeroTemperature_IfOff()
        {
            var data = new TestHeaterData
            {
                Temperature = 22.5f,
                IsOn = false
            };
            var logic = new DummyHeaterLogic(data);

            Assert.AreEqual(0f, logic.Temperature);
        }
    }
}