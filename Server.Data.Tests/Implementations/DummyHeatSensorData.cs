using TPUM.Server.Data.Events;

namespace TPUM.Server.Data.Tests
{
    internal class DummyHeatSensorData : IHeatSensorData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly object _sensorLock = new();

        public Guid Id { get; }

        private DummyPositionData _position;
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

        public DummyHeatSensorData(Guid id, float x, float y)
        {
            Id = id;
            _position = new DummyPositionData(x, y);
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