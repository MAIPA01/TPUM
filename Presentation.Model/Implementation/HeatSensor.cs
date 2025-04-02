using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TPUM.Presentation.Model
{
    internal class HeatSensor : IHeatSensor
    {
        private readonly Logic.IHeatSensor _sensor;

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

        public HeatSensor(Logic.IHeatSensor sensor)
        {
            _sensor = sensor;
            _sensor.PositionChanged += GetPositionChanged;
            _sensor.TemperatureChanged += GetTemperatureChanged;
        }

        private void GetPositionChanged(object? source, Logic.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), Position));
        }

        private void GetTemperatureChanged(object? source, Logic.TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
        }

        public void Dispose()
        {
            _sensor.PositionChanged += GetPositionChanged;
            _sensor.TemperatureChanged += GetTemperatureChanged;
            GC.SuppressFinalize(this);
        }
    }
}