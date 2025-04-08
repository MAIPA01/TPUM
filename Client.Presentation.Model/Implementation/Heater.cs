using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    internal class Heater : IHeater
    {
        private readonly IHeaterLogic _logic;

        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id => _logic.Id;
        public IPosition Position
        {
            get => new Position(_logic.Position);
            set
            {
                _logic.Position.X = value.X;
                _logic.Position.Y = value.Y;
            }
        }

        public float Temperature
        {
            get => _logic.Temperature;
            set => _logic.Temperature = value;
        }

        public bool IsOn => _logic.IsOn;

        public Heater(IHeaterLogic logic)
        {
            _logic = logic;
            _logic.TemperatureChanged += GetTemperatureChanged;
            _logic.PositionChanged += GetPositionChanged;
            _logic.EnableChanged += GetEnableChanged;
        }

        private void GetPositionChanged(object? source, Logic.Events.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), Position));
        }

        private void GetPositionChanged(object? source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(args.LastPosition, Position));
        }

        private void GetTemperatureChanged(object? source, Logic.Events.TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
        }

        private void GetTemperatureChanged(object? source, TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
        }

        private void GetEnableChanged(object? source, Logic.Events.EnableChangedEventArgs args)
        {
            EnableChanged?.Invoke(this, new EnableChangedEventArgs(args.LastEnable, args.NewEnable));
        }

        private void GetEnableChanged(object? source, EnableChangedEventArgs args)
        {
            EnableChanged?.Invoke(this, new EnableChangedEventArgs(args.LastEnable, args.NewEnable));
        }

        public void TurnOn()
        {
            _logic.TurnOn();
        }

        public void TurnOff()
        {
            _logic.TurnOff();
        }

        public void Dispose()
        {
            _logic.TemperatureChanged -= GetTemperatureChanged;
            _logic.PositionChanged -= GetPositionChanged;
            _logic.EnableChanged -= GetEnableChanged;
            _logic.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}