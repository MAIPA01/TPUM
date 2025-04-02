using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TPUM.Presentation.ViewModel
{
    internal class HeatSensor : IHeatSensor
    {
        private readonly Model.IHeatSensor _sensor;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

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

        public HeatSensor(Model.IHeatSensor sensor)
        {
            _sensor = sensor;
            _sensor.PositionChanged += GetPositionChanged;
            _sensor.TemperatureChanged += GetTemperatureChanged;
        }

        private void GetPositionChanged(object? source, Model.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), Position));
            OnPropertyChange(nameof(Position));
        }

        private void GetTemperatureChanged(object? source, Model.TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
            OnPropertyChange(nameof(Temperature));
        }

        public void Dispose()
        {
            _sensor.PositionChanged -= GetPositionChanged;
            _sensor.TemperatureChanged -= GetTemperatureChanged;
            GC.SuppressFinalize(this);
        }

        protected void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}