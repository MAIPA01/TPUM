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
                    var lastIsOn = _isOn;
                    _isOn = value;
                    EnableChanged?.Invoke(this, lastIsOn, _isOn);
                    _data.UpdateHeater(_roomId, Id, _position.X, _position.Y, _temperature, _isOn);
                }
            }
        }

        private IPositionData _position;
        public IPositionData Position
        {
            get
            {
                lock (_heaterLock)
                {
                    return _position;
                }
            }
            set
            {
                lock (_heaterLock)
                {
                    if (_position.Equals(value)) return;
                    var lastPosition = _position;
                    _position.PositionChanged -= GetPositionChanged;
                    _position = value;
                    _position.PositionChanged += GetPositionChanged;
                    PositionChanged?.Invoke(this, lastPosition, _position);
                    _data.UpdateHeater(_roomId, Id, _position.X, _position.Y, _temperature, _isOn);
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
                    var lastTemperature = _temperature;
                    _temperature = value;
                    TemperatureChanged?.Invoke(this, lastTemperature, _temperature);
                    _data.UpdateHeater(_roomId, Id, _position.X, _position.Y, _temperature, _isOn);
                }
            }
        }

        public HeaterData(DataApi data, Guid roomId, Guid id, IPositionData position, float temperature, bool isOn = false)
        {
            _data = data;
            _roomId = roomId;
            Id = id;
            _position = position;
            _position.PositionChanged += GetPositionChanged;
            _temperature = temperature;
            _isOn = isOn;
        }

        private void GetPositionChanged(object? source, IPositionData lastPosition, IPositionData newPosition)
        {
            PositionChanged?.Invoke(this, lastPosition, newPosition);
            _data.UpdateHeater(_roomId, Id, Position.X, Position.Y, Temperature, IsOn);
        }

        public void Dispose()
        {
            _position.PositionChanged -= GetPositionChanged;
            GC.SuppressFinalize(this);
        }
    }
}