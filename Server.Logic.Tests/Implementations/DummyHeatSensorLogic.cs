using TPUM.Server.Data;
using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic.Tests
{
    internal class DummyHeatSensorLogic : IHeatSensorLogic
    {
        private readonly IHeatSensorData _data;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public Guid Id => _data.Id;
        private readonly DummyPositionLogic _position;
        public IPositionLogic Position => _position;
        public float Temperature => _data.Temperature;

        public DummyHeatSensorLogic(IHeatSensorData data)
        {
            _data = data;
            _data.PositionChanged += GetPositionChanged;
            _data.TemperatureChanged += GetTemperatureChanged;

            _position = new DummyPositionLogic(_data.Position);
        }

        public void SetPosition(float x, float y)
        {
            _position.SetPosition(x, y);
        }

        internal void SetTemperature(float temperature)
        {
            _data.Temperature = temperature;
        }

        private void GetPositionChanged(object? source, IPositionData lastPosition, IPositionData newPosition)
        {
            PositionChanged?.Invoke(this, new DummyPositionLogic(lastPosition), new DummyPositionLogic(newPosition));
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, lastTemperature, newTemperature);
        }

        public void Dispose()
        {
            _data.PositionChanged -= GetPositionChanged;
            _data.TemperatureChanged -= GetTemperatureChanged;
            GC.SuppressFinalize(this);
        }

        public override int GetHashCode()
        {
            return 3 * Position.GetHashCode() + 5 * Temperature.GetHashCode();
        }
    }
}