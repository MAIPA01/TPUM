namespace TPUM.Client.Presentation.Model
{
    internal class Heater : IHeater
    {
        private readonly Logic.IHeater _heater;

        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public long Id => _heater.Id;
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

        public bool IsOn => _heater.IsOn;

        public Heater(Logic.IHeater heater)
        {
            _heater = heater;
            _heater.TemperatureChanged += GetTemperatureChanged;
            _heater.PositionChanged += GetPositionChanged;
            _heater.EnableChanged += GetEnableChanged;
        }

        private void GetPositionChanged(object? source, Logic.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), Position));
        }

        private void GetTemperatureChanged(object? source, Logic.TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
        }

        private void GetEnableChanged(object? source, Logic.EnableChangedEventArgs args)
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
            _heater.TemperatureChanged += GetTemperatureChanged;
            _heater.PositionChanged += GetPositionChanged;
            _heater.EnableChanged += GetEnableChanged;
            GC.SuppressFinalize(this);
        }
    }
}