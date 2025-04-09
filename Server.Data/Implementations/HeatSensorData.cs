using TPUM.Server.Data.Events;

namespace TPUM.Server.Data
{
    internal class HeatSensorData : IHeatSensorData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id { get; }

        private IPositionData _position;
        private readonly object _positionLock = new();

        public IPositionData Position
        {
            get
            {
                lock (_positionLock)
                {
                    return _position;
                }
            }
            set
            {
                lock (_positionLock)
                {
                    if (_position.Equals(value)) return;
                    var lastPosition = _position;
                    _position.PositionChanged -= GetPositionChange;
                    _position = value;
                    _position.PositionChanged += GetPositionChange;
                    PositionChanged?.Invoke(this, lastPosition, _position);
                }
            }
        }

        private float _temperature = 0.0f;
        private readonly object _temperatureLock = new();

        public float Temperature
        {
            get
            {
                lock (_temperatureLock)
                {
                    return _temperature;
                }
            }
            set
            {
                lock (_temperatureLock)
                {
                    if (Math.Abs(_temperature - value) < 1e-10f) return;
                    var lastTemperature = _temperature;
                    _temperature = value;
                    TemperatureChanged?.Invoke(this, lastTemperature, _temperature);
                }
            }
        }

        public HeatSensorData(Guid id, IPositionData position)
        {
            Id = id;
            _position = position;
            _position.PositionChanged += GetPositionChange;
        }

        private void GetPositionChange(object? source, IPositionData lastPosition, IPositionData newPosition)
        {
            PositionChanged?.Invoke(this, lastPosition, newPosition);
        }

        public void Dispose()
        {
            _position.PositionChanged -= GetPositionChange;
            GC.SuppressFinalize(this);
        }
    }
}