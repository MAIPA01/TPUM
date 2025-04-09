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

        private readonly object _posLock = new();
        private IPositionLogic _position;
        public IPositionLogic Position
        {
            get
            {
                lock (_posLock)
                {
                    return _position;
                }
            }
            set
            {
                lock (_posLock)
                {
                    if (_position.Equals(value)) return;
                    IPositionLogic last = new PositionLogic(DataApiBase.GetApi().CreatePosition(_position.X, _position.Y));
                    _position.PositionChanged -= GetPositionChanged;
                    // By nie wywolaly sie eventy 2 razy
                    _position.X = value.X;
                    _position.Y = value.Y;
                    _position.PositionChanged += GetPositionChanged;
                    OnPositionChanged(this, last);
                }
            }
        }

        private readonly object _tempLock = new();
        public float Temperature => _data.Temperature;

        public HeatSensorLogic(IHeatSensorData data)
        {
            _data = data;
            _position = new PositionLogic(_data.Position);
            _position.PositionChanged += GetPositionChanged;
        }

        internal void SetTemperature(float temperature)
        {
            lock (_tempLock)
            {
                if (Math.Abs(_data.Temperature - temperature) < 1e-10f) return;
                float last = _data.Temperature;
                _data.Temperature = temperature;
                OnTemperatureChanged(this, last);
            }
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
        {
            PositionChanged?.Invoke(this, lastPosition, newPosition);
        }

        public void Dispose()
        {
            _position.PositionChanged -= GetPositionChanged;
            _position.Dispose();
            GC.SuppressFinalize(this);
        }

        public override int GetHashCode()
        {
            return 3 * Position.GetHashCode() + 5 * Temperature.GetHashCode();
        }

        private void OnPositionChanged(object? source, IPositionLogic lastPosition)
        {
            PositionChanged?.Invoke(source, lastPosition, _position);
        }

        private void OnTemperatureChanged(object? source, float lastTemperature)
        {
            TemperatureChanged?.Invoke(source, lastTemperature, _data.Temperature);
        }
    }
}