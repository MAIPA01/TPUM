using TPUM.Server.Data.Events;

namespace TPUM.Server.Data.Tests
{
    internal class DummyHeaterData : IHeaterData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id { get; }

        private bool _isOn;
        private readonly object _isOnLock = new();

        public bool IsOn
        {
            get
            {
                lock (_isOnLock)
                {
                    return _isOn;
                }
            }
            set
            {
                lock (_isOnLock)
                {
                    if (_isOn == value) return;
                    var lastEnable = _isOn;
                    _isOn = value;
                    EnableChanged?.Invoke(this, lastEnable, _isOn);
                }
            }
        }

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

        private float _temperature;
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

        public DummyHeaterData(Guid id, IPositionData position, float temperature, bool isOn = false)
        {
            Id = id;
            _position = position;
            _position.PositionChanged += GetPositionChange;
            _temperature = temperature;
            _isOn = isOn;
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