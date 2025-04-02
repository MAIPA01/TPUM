using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TPUM.Presentation.Model
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

        public ICommand TurnOnCommand { get; }
        public ICommand TurnOffCommand { get; }

        public Heater(Logic.IHeater heater)
        {
            _heater = heater;
            _heater.TemperatureChanged += GetTemperatureChanged;
            _heater.PositionChanged += GetPositionChanged;
            _heater.EnableChanged += GetEnableChanged;

            TurnOnCommand = new CustomCommand(TurnOn);
            TurnOffCommand = new CustomCommand(TurnOff);
        }

        private void GetPositionChanged(object source, Logic.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), Position));
        }

        private void GetTemperatureChanged(object source, Logic.TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
        }

        private void GetEnableChanged(object source, Logic.EnableChangedEventArgs args)
        {
            EnableChanged?.Invoke(this, new EnableChangedEventArgs(args.LastEnable, args.NewEnable));
        }

        private void TurnOn(object? param)
        {
            _heater.TurnOn();
        }

        private void TurnOff(object? param)
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
