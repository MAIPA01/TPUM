using TPUM.Server.Data;
using TPUM.Server.Data.Events;

namespace TPUM.Server.Logic.Tests
{
    [TestClass]
    public sealed class HeatSensorLogicTest
    {
        public class HeatSensorData : IHeatSensorData
        {
            public Guid Id { get; } = Guid.NewGuid();
            public float Temperature { get; set; }
            public IPositionData Position { get; } = new PositionData();
            IPositionData IHeatSensorData.Position { get => Position; set => new PositionData(value.X, value.Y); }

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

            event PositionChangedEventHandler? INotifyPositionChanged.PositionChanged
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
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
        public void HeatSensorLogic_Temperature_ReturnsSetValue()
        {
            var data = new HeatSensorData();
            var logic = new DummyHeatSensorLogic(data);

            logic.SetTemperature(21.5f);

            Assert.AreEqual(21.5f, logic.Temperature);
        }
    }
}
