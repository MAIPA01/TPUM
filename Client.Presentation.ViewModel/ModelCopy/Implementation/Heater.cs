using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TPUM.Client.Presentation.ViewModel
{
    internal class Heater : IHeater
    {
        private readonly Model.IHeater _heater;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangeEventHandler? EnableChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Guid Id => _heater.Id;

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

        public ICommand TurnCommand => IsOn ? _turnOffCommand : _turnOnCommand;
        private readonly ICommand _turnOffCommand;
        private readonly ICommand _turnOnCommand;

        public Heater(Model.IHeater heater)
        {
            _heater = heater;
            _heater.PositionChanged += GetPositionChanged;
            _heater.TemperatureChanged += GetTemperatureChanged;
            _heater.EnableChanged += GetEnableChanged;

            _turnOffCommand = new CustomCommand(_ => _heater.TurnOff());
            _turnOnCommand = new CustomCommand(_ => _heater.TurnOn());
        }

        private void GetPositionChanged(object? source, Model.Events.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(args.LastX, args.LastY, Position.X, Position.Y));
            OnPropertyChange(nameof(Position));
        }

        private void GetTemperatureChanged(object? source, Model.Events.TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
            OnPropertyChange(nameof(Temperature));
        }

        private void GetEnableChanged(object? source, Model.Events.EnableChangedEventArgs args)
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