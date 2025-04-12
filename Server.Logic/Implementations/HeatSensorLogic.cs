using TPUM.Server.Data;
using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
{
    internal class HeatSensorLogic : IHeatSensorLogic
    {
        private readonly IHeatSensorData _data;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id => _data.Id;

        private readonly PositionLogic _position;
        public IPositionLogic Position => _position;
        public float Temperature => _data.Temperature;

        public HeatSensorLogic(IHeatSensorData data)
        {
            _data = data;
            _data.PositionChanged += GetPositionChanged;
            _data.TemperatureChanged += GetTemperatureChanged;

            _position = new PositionLogic(_data.Position);
        }

        internal void SetTemperature(float temperature)
        {
            _data.Temperature = temperature;
        }

        private void GetPositionChanged(object? source, IPositionData lastPosition, IPositionData newPosition)
        {
            PositionChanged?.Invoke(this, new PositionLogic(lastPosition), new PositionLogic(newPosition));
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, lastTemperature, newTemperature);
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
            _data.PositionChanged -= GetPositionChanged;
            _data.TemperatureChanged -= GetTemperatureChanged;
            GC.SuppressFinalize(this);
        }
    }
}