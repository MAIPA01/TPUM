namespace TPUM.Client.Data.Tests
{
    internal class DummyHeater : IHeater
    {
        public long Id { get; }

        private bool _isOn = false;
        public bool IsOn
        {
            get => _isOn;
            private set
            {
                if (_isOn == value) return;
                lock (_isOnLock)
                {
                    _isOn = value;
                    OnEnableChanged(this, !_isOn);
                }
            }
        }

        private IPosition _position;
        public IPosition Position
        {
            get => _position;
            set
            {
                if (Equals(_position, value)) return;
                lock (_positionLock)
                {
                    var lastPosition = _position;
                    _position.PositionChanged -= GetPositionChanged;
                    _position = value;
                    _position.PositionChanged += GetPositionChanged;
                }
            }
        }

        private float _temperature;
        public float Temperature
        {
            get => IsOn ? _temperature : 0f;
            set
            {
                if (Math.Abs(_temperature - value) < 1e-10f) return;
                lock (_temperatureLock)
                {
                    var lastTemperature = _temperature;
                    _temperature = value;
                    OnTemperatureChanged(this, lastTemperature);
                }
            }
        }

        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly object _isOnLock = new();
        private readonly object _positionLock = new();
        private readonly object _temperatureLock = new();

        public DummyHeater(long id, float x, float y, float temperature)
        {
            Id = id;
            _position = new DummyPosition(x, y);
            _position.PositionChanged += GetPositionChanged;
            _temperature = temperature;
        }

        private void GetPositionChanged(object? source, PositionChangedEventArgs e)
        {
            PositionChanged?.Invoke(this, e);
        }

        public void TurnOff()
        {
            IsOn = false;
        }

        public void TurnOn()
        {
            IsOn = true;
        }

        public void Dispose()
        {
            _position.PositionChanged -= GetPositionChanged;
            _position.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnEnableChanged(object? source, bool lastEnable)
        {
            EnableChanged?.Invoke(source, new EnableChangedEventArgs(lastEnable, _isOn));
        }

        private void OnTemperatureChanged(object? source, float lastTemperature)
        {
            TemperatureChanged?.Invoke(source, new TemperatureChangedEventArgs(lastTemperature, _temperature));
        }
    }
}