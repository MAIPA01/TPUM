using TPUM.Server.Data;
using TPUM.Server.Data.Events;

namespace TPUM.Server.Logic.Tests
{
    [TestClass]
    public sealed class HeaterLogicTest
    {
        public class HeaterData : IHeaterData
        {
            public Guid Id { get; } = Guid.NewGuid();
            public bool IsOn { get; set; }
            public float Temperature { get; set; }
            public IPositionData Position { get; } = new PositionData();
            IPositionData IHeaterData.Position { get => Position; set => new PositionData(value.X, value.Y); }

            public event EnableChangedEventHandler? EnableChanged;
            public event TemperatureChangedEventHandler? TemperatureChanged;
            public event PositionChangedEventHandler? PositionChanged;

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        public class PositionData : IPositionData
        {
            public float X { get; set; }
            public float Y { get; set; }

            public PositionData() { }
            public PositionData(float x, float y)
            {
                X = x;
                Y = y;
            }

            public void SetPosition(float x, float y)
            {
                X = x;
                Y = y;
                PositionChanged?.Invoke(this, this, this);
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public event PositionChangedEventHandler? PositionChanged;
        }

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
