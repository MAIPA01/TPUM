using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    internal class HeaterLogic : IHeaterLogic
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;

        private readonly IHeaterData _data;

        public Guid Id => _data.Id;

        public bool IsOn => _data.IsOn;

        public IPositionLogic Position
        {
            get => new PositionLogic(_data.Position);
            set => _data.Position.SetPosition(value.X, value.Y);
        }

        public float Temperature
        {
            get => _data.Temperature;
            set => _data.Temperature = value;
        }

        public HeaterLogic(IHeaterData data)
        {
            _data = data;
            _data.PositionChanged += GetPositionChanged;
            _data.TemperatureChanged += GetTemperatureChanged;
            _data.EnableChanged += GetEnableChanged;
        }

        private void GetPositionChanged(object? source, IPositionData lastPosition, IPositionData newPosition)
        {
            PositionChanged?.Invoke(this, new PositionLogic(lastPosition), new PositionLogic(newPosition));
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, lastTemperature, newTemperature);
        }

        private void GetEnableChanged(object? source, bool lastEnable, bool newEnable)
        {
            EnableChanged?.Invoke(this, lastEnable, newEnable);
        }

        public void TurnOn()
        {
            _data.IsOn = true;
        }

        public void TurnOff()
        {
            _data.IsOn = false;
        }

        public void Dispose()
        {
            _data.PositionChanged -= GetPositionChanged;
            _data.TemperatureChanged -= GetTemperatureChanged;
            _data.EnableChanged -= GetEnableChanged;
            GC.SuppressFinalize(this);
        }
    }
}