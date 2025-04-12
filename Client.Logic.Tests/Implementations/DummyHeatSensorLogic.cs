using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic.Tests
{
    internal class DummyHeatSensorLogic : IHeatSensorLogic
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly IHeatSensorData _data;

        public Guid Id => _data.Id;

        private readonly DummyPositionLogic _position;
        public IPositionLogic Position => _position;

        public float Temperature => _data.Temperature;

        public DummyHeatSensorLogic(IHeatSensorData data)
        {
            _data = data;
            _position = new DummyPositionLogic(_data.Position);
        }

        public void SetPosition(float x, float y)
        {
            _data.SetPosition(x, y);
        }

        public override int GetHashCode()
        {
            return 3 * Position.GetHashCode() + 5 * Temperature.GetHashCode();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}