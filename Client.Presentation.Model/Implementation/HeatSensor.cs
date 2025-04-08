using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    internal class HeatSensor : IHeatSensor
    {
        private readonly IHeatSensorLogic _logic;

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
        public float Temperature => _logic.Temperature;

        public HeatSensor(IHeatSensorLogic logic)
        {
            _logic = logic;
            _logic.PositionChanged += GetPositionChanged;
            _logic.TemperatureChanged += GetTemperatureChanged;
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

        public void Dispose()
        {
            _logic.PositionChanged -= GetPositionChanged;
            _logic.TemperatureChanged -= GetTemperatureChanged;
            _logic.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}