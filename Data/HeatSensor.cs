namespace TPUM.Data
{
    internal class HeatSensor : IHeatSensor
    {
        public long Id { get; }

        private Position _position;
        public Position Position
        {
            get => _position;
            set
            {
                if (_position == value) return;
                lock (_positionLock)
                {
                    Position lastPosition = _position;
                    _position.PositionChanged -= GetPositionChanged;
                    _position = value;
                    _position.PositionChanged += GetPositionChanged;
                    OnPositionChanged(this, lastPosition);
                }
            }
        }

        private float _temperature = .0f;
        public float Temperature
        {
            get => _temperature;
            set
            {
                if (Math.Abs(_temperature - value) < 1e-10f) return;
                lock (_temperatureLock)
                {
                    float lastTemperature = _temperature;
                    _temperature = value;
                    OnTemperatureChanged(this, lastTemperature);
                }
            }
        }

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly object _positionLock = new();
        private readonly object _temperatureLock = new();

        public HeatSensor(long id, float x, float y)
        {
            Id = id;
            _position = new(x, y);
            _position.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object source, PositionChangedEventArgs e)
        {
            PositionChanged?.Invoke(this, e);
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

        private void OnPositionChanged(object source, Position lastPosition)
        {
            PositionChanged?.Invoke(source, new PositionChangedEventArgs(lastPosition, _position));
        }

        private void OnTemperatureChanged(object source, float lastTemperature)
        {
            TemperatureChanged?.Invoke(source, new TemperatureChangedEventArgs(lastTemperature, _temperature));
        }
    }
}
