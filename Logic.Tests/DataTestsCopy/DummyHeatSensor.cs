namespace TPUM.Logic.Tests
{
    internal class DummyHeatSensor : IHeatSensor
    {
        private readonly Data.IHeatSensor _sensor;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public long Id => _sensor.Id;

        public IPosition Position
        {
            get => new Position(_sensor.Position);
            set
            {
                _sensor.Position.X = value.X;
                _sensor.Position.Y = value.Y;
            }
        }

        public float Temperature => _sensor.Temperature;

        public DummyHeatSensor(Data.IHeatSensor sensor)
        {
            _sensor = sensor;
            _sensor.PositionChanged += GetPositionChanged;
            _sensor.TemperatureChanged += GetTemperatureChanged;
        }

        internal void SetTemperature(float temperature)
        {
            _sensor.Temperature = temperature;
        }

        private void GetPositionChanged(object? source, Data.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), Position));
        }

        private void GetTemperatureChanged(object? source, Data.TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(source, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
        }

        public void Dispose()
        {
            _sensor.PositionChanged -= GetPositionChanged;
            _sensor.TemperatureChanged -= GetTemperatureChanged;
            GC.SuppressFinalize(this);
        }
    }
}