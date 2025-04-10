using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    internal class HeatSensorData : IHeatSensorData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly DataApi _data;
        private readonly Guid _roomId;

        private readonly object _heatSensorLock = new();

        public Guid Id { get; }

        private IPositionData _position;
        public IPositionData Position
        {
            get 
            {
                lock (_heatSensorLock)
                {
                    return _position;
                }
            }
            set
            {
                lock (_heatSensorLock)
                {
                    if (_position.Equals(value)) return;
                    var lastPosition = _position;
                    _position.PositionChanged -= GetPositionChanged;
                    _position = value;
                    _position.PositionChanged += GetPositionChanged;
                    PositionChanged?.Invoke(this, lastPosition, _position);
                    _data.UpdateHeatSensor(_roomId, Id, _position.X, _position.Y);
                }
            }
        }
        private float _temperature;
        public float Temperature
        {
            get
            {
                lock (_heatSensorLock)
                {
                    return _temperature;
                }
            }
            private set
            {
                lock (_heatSensorLock)
                {
                    if (MathF.Abs(_temperature - value) < 1e-10f) return;
                    var lastTemperature = _temperature;
                    _temperature = value;
                    TemperatureChanged?.Invoke(this, lastTemperature, _temperature);
                }
            }
        }

        public HeatSensorData(DataApi data, Guid roomId, Guid id, IPositionData position, float temperature = 0.0f)
        {
            _data = data;
            _roomId = roomId;
            Id = id;
            _position = position;
            _position.PositionChanged += GetPositionChanged;
            Temperature = temperature;
        }

        private void GetPositionChanged(object? source, IPositionData lastPosition, IPositionData newPosition)
        {
            PositionChanged?.Invoke(this, lastPosition, newPosition);
            _data.UpdateHeatSensor(_roomId, Id, _position.X, _position.Y);
        }

        internal void SetTemperature(float temperature)
        {
            Temperature = temperature;
        }

        public void Dispose()
        {
            _position.PositionChanged -= GetPositionChanged;
            GC.SuppressFinalize(this);
        }
    }
}