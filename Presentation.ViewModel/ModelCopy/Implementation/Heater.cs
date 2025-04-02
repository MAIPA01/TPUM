using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TPUM.Presentation.ViewModel
{
    internal class Heater : IHeater
    {
        private readonly Model.IHeater _heater;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangeEventHandler? EnableChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

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

        public string TurnText => IsOn ? "Turn Off" : "Turn On";

        public ICommand TurnCommand => IsOn ? TurnOffCommand : TurnOnCommand;
        private ICommand TurnOffCommand => new CustomCommand(_heater.TurnOffCommand);
        private ICommand TurnOnCommand => new CustomCommand(_heater.TurnOnCommand);

        public Heater(Model.IHeater heater)
        {
            _heater = heater;
            _heater.PositionChanged += GetPositionChanged;
            _heater.TemperatureChanged += GetTemperatureChanged;
            _heater.EnableChanged += GetEnableChanged;
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

        private void GetEnableChanged(object? source, Model.EnableChangedEventArgs args)
        {
            EnableChanged?.Invoke(this, new EnableChangeEventArgs(args.LastEnable, args.NewEnable));
            OnPropertyChange(nameof(IsOn));
            OnPropertyChange(nameof(Temperature));
            OnPropertyChange(nameof(TurnText));
            OnPropertyChange(nameof(TurnCommand));
        }

        public void Dispose()
        {
            _heater.PositionChanged -= GetPositionChanged;
            _heater.TemperatureChanged -= GetTemperatureChanged;
            _heater.EnableChanged -= GetEnableChanged;
            GC.SuppressFinalize(this);
        }

        protected void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}