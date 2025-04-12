using TPUM.Server.Data.Events;

namespace TPUM.Server.Data.Tests
{
    internal class DummyHeaterData : IHeaterData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly object _heaterLock = new();

        public Guid Id { get; }

        private bool _isOn;
        public bool IsOn
        {
            get
            {
                lock (_heaterLock)
                {
                    return _isOn;
                }
            }
            set
            {
                lock (_heaterLock)
                {
                    if (_isOn == value) return;
                    var lastEnable = _isOn;
                    _isOn = value;
                    EnableChanged?.Invoke(this, lastEnable, _isOn);
                }
            }
        }

        private readonly DummyPositionData _position;
        public IPositionData Position
        {
            get
            {
                lock (_heaterLock)
                {
                    return _position;
                }
            }
        }

        private float _temperature;
        public float Temperature
        {
            get
            {
                lock (_heaterLock)
                {
                    return _temperature;
                }
            }
            set
            {
                lock (_heaterLock)
                {
                    if (Math.Abs(_temperature - value) < 1e-10f) return;
                    var lastTemperature = _temperature;
                    _temperature = value;
                    TemperatureChanged?.Invoke(this, lastTemperature, _temperature);
                }
            }
        }

        public DummyHeaterData(Guid id, float x, float y, float temperature, bool isOn = false)
        {
            Id = id;
            _position = new DummyPositionData(x, y);
            _temperature = temperature;
            _isOn = isOn;
        }

        public void SetPosition(float x, float y)
        {
            var lastPosition = new DummyPositionData(_position.X, _position.Y);
            _position.X = x;
            _position.Y = y;
            PositionChanged?.Invoke(this, lastPosition, _position);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}