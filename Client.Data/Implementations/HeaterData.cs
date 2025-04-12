using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    internal class HeaterData : IHeaterData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly DataApi _data;
        private readonly Guid _roomId;

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
                    _data.UpdateHeater(_roomId, Id, _position.X, _position.Y, _temperature, value);
                }
            }
        }

        private readonly PositionData _position;
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
                    if (MathF.Abs(_temperature - value) < 1e-10f) return;
                    _data.UpdateHeater(_roomId, Id, _position.X, _position.Y, value, _isOn);
                }
            }
        }

        public HeaterData(DataApi data, Guid roomId, Guid id, PositionData position, float temperature, bool isOn = false)
        {
            _data = data;
            _roomId = roomId;
            Id = id;
            _position = position;
            _temperature = temperature;
            _isOn = isOn;
        }

        public void SetPosition(float x, float y)
        {
            lock (_heaterLock)
            {
                if (MathF.Abs(_position.X - x) < 1e-10f && MathF.Abs(_position.Y - y) < 1e-10f) return;
                _data.UpdateHeater(_roomId, Id, x, y, _temperature, _isOn);
            }
        }

        internal void UpdateDataFromServer(float x, float y, float temperature, bool isOn)
        {
            lock (_heaterLock)
            {
                if (MathF.Abs(_position.X - x) > 1e-10f || MathF.Abs(_position.Y - y) > 1e-10f)
                {
                    var lastPosition = new PositionData(_position.X, _position.Y);
                    _position.SetPosition(x, y);
                    PositionChanged?.Invoke(this, lastPosition, _position);
                }

                if (Math.Abs(_temperature - temperature) > 1e-10f)
                {
                    var lastTemperature = _temperature;
                    _temperature = temperature;
                    TemperatureChanged?.Invoke(this, lastTemperature, _temperature);
                }

                if (_isOn != isOn)
                {
                    var lastIsOn = _isOn;
                    _isOn = isOn;
                    EnableChanged?.Invoke(this, lastIsOn, _isOn);
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}