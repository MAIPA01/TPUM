using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TPUM.Data;

namespace TPUM.Presentation.Model
{
    internal class ModelHeater : IModelHeater
    {
        private readonly IHeater _heater;

        public event EnableChangeEventHandler? EnableChange;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public long Id => _heater.Id;
        public IPosition Position
        {
            get => _heater.Position;
            set => _heater.Position = value;
        }

        public float Temperature
        {
            get => _heater.Temperature;
            set => _heater.Temperature = value;
        }

        public bool IsOn => _heater.IsOn;

        public ModelHeater(IHeater heater)
        {
            _heater = heater;
            _heater.TemperatureChanged += GetTemperatureChanged;
            _heater.PositionChanged += GetPositionChanged;
            _heater.EnableChange += GetEnableChanged;
        }

        private void GetPositionChanged(object source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, args);
            OnPropertyChange(nameof(Position));
        }

        private void GetTemperatureChanged(object source, TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(this, args);
            OnPropertyChange(nameof(Temperature));
        }

        private void GetEnableChanged(object source, EnableChangeEventArgs args)
        {
            EnableChange?.Invoke(this, args);
            OnPropertyChange(nameof(IsOn));
            OnPropertyChange(nameof(Temperature));
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
            _heater.EnableChange += GetEnableChanged;
            _heater.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
