using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    internal class HeatSensorLogic : IHeatSensorLogic
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly IHeatSensorData _data;

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

        public void Dispose()
        {
            _data.PositionChanged -= GetPositionChanged;
            _data.TemperatureChanged -= GetTemperatureChanged;
            GC.SuppressFinalize(this);
        }
    }
}