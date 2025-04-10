using TPUM.Client.Presentation.ViewModel.Events;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TPUM.Client.Presentation.Model;

namespace TPUM.Client.Presentation.ViewModel
{
    internal class Heater : IHeater
    {
        private readonly IHeaterModel _model;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangeEventHandler? EnableChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Guid Id => _model.Id;

        public IPosition Position
        {
            get => new Position(_model.Position);
            set => _model.Position.SetPosition(value.X, value.Y);
        }

        public float CurrentTemperature => IsOn ? _model.Temperature : 0f;

        public float DesiredTemperature
        {
            get => _model.Temperature;
            set => _model.Temperature = value;
        }

        public bool IsOn => _model.IsOn;

        public string TurnText => IsOn ? "Turn Off" : "Turn On";

        public ICommand TurnCommand => IsOn ? _turnOffCommand : _turnOnCommand;
        private readonly ICommand _turnOffCommand;
        private readonly ICommand _turnOnCommand;

        public Heater(IHeaterModel model)
        {
            _model = model;
            _model.PositionChanged += GetPositionChanged;
            _model.TemperatureChanged += GetTemperatureChanged;
            _model.EnableChanged += GetEnableChanged;

            _turnOffCommand = new CustomCommand(_ => _model.TurnOff());
            _turnOnCommand = new CustomCommand(_ => _model.TurnOn());
        }

        private void GetPositionChanged(object? source, IPositionModel lastPosition, IPositionModel newPosition)
        {
            PositionChanged?.Invoke(this, new Position(lastPosition), new Position(newPosition));
            OnPropertyChange(nameof(Position));
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, lastTemperature, newTemperature);
            OnPropertyChange(nameof(CurrentTemperature));
            OnPropertyChange(nameof(DesiredTemperature));
        }

        private void GetEnableChanged(object? source, bool lastEnable, bool newEnable)
        {
            EnableChanged?.Invoke(this, lastEnable, newEnable);
            OnPropertyChange(nameof(IsOn));
            OnPropertyChange(nameof(CurrentTemperature));
            OnPropertyChange(nameof(DesiredTemperature));
            OnPropertyChange(nameof(TurnText));
            OnPropertyChange(nameof(TurnCommand));
        }

        public void Dispose()
        {
            _model.PositionChanged -= GetPositionChanged;
            _model.TemperatureChanged -= GetTemperatureChanged;
            _model.EnableChanged -= GetEnableChanged;
            GC.SuppressFinalize(this);
        }

        protected void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}