namespace TPUM.Client.Logic.Tests
{
    internal class DummyHeater : IHeater
    {
        private readonly Data.IHeater _heater;

        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public long Id => _heater.Id;
        public bool IsOn => _heater.IsOn;

        public IPosition Position
        {
            get => new Position(_heater.Position);
            set
            {
                _heater.Position.X = value.X;
                _heater.Position.Y = value.Y;
            }
        }

        public float Temperature
        {
            get => _heater.Temperature;
            set => _heater.Temperature = value;
        }

        public DummyHeater(Data.IHeater heater)
        {
            _heater = heater;
            _heater.PositionChanged += GetPositionChanged;
            _heater.TemperatureChanged += GetTemperatureChanged;
            _heater.EnableChanged += GetEnableChanged;
        }

        private void GetPositionChanged(object? source, Data.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), Position));
        }

        private void GetTemperatureChanged(object? source, Data.TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
        }

        private void GetEnableChanged(object? source, Data.EnableChangedEventArgs args)
        {
            EnableChanged?.Invoke(this, new EnableChangedEventArgs(args.LastEnable, args.NewEnable));
        }

        public void TurnOn()
        {
            _heater.TurnOn();
        }

        public void TurnOff()
        {
            _heater.TurnOff();
        }

        public void Dispose()
        {
            _heater.PositionChanged -= GetPositionChanged;
            _heater.TemperatureChanged -= GetTemperatureChanged;
            _heater.EnableChanged -= GetEnableChanged;
            GC.SuppressFinalize(this);
        }
    }
}