using TPUM.Server.Data.Events;

namespace TPUM.Server.Data
{
    internal class HeatSensorData : IHeatSensorData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly object _sensorLock = new();

        public Guid Id { get; }

        private readonly PositionData _position;
        public IPositionData Position
        {
            get
            {
                lock (_sensorLock)
                {
                    return _position;
                }
            }
        }

        private float _temperature = 0.0f;
        public float Temperature
        {
            get
            {
                lock (_sensorLock)
                {
                    return _temperature;
                }
            }
            set
            {
                lock (_sensorLock)
                {
                    if (Math.Abs(_temperature - value) < 1e-10f) return;
                    var lastTemperature = _temperature;
                    _temperature = value;
                    TemperatureChanged?.Invoke(this, lastTemperature, _temperature);
                }
            }
        }

        public HeatSensorData(Guid id, PositionData position)
        {
            Id = id;
            _position = position;
        }

        public void SetPosition(float x, float y)
        {
            lock (_sensorLock)
            {
                if (Math.Abs(_position.X - x) < 1e-10f && Math.Abs(_position.Y - y) < 1e-10f) return;
                var lastPosition = new PositionData(_position.X, _position.Y);
                _position.SetPosition(x, y);
                PositionChanged?.Invoke(this, lastPosition, _position);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}