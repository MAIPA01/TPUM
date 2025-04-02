using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TPUM.Presentation.ViewModel
{
    internal class Heater : IHeater
    {
        private readonly Model.IHeater _heater;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangeEventHandler? EnableChange;
        public event PropertyChangedEventHandler? PropertyChanged;

        public long Id => _heater.Id;

        public IPosition Position 
        { 
            get => new Position(_heater.Position);
            set
            {
                if (_heater.Position != value)
                {
                    _heater.Position.X = value.X;
                    _heater.Position.Y = value.Y;
                    OnPropertyChange(nameof(Position));
                }
            }
        }
        public float Temperature 
        { 
            get => _heater.Temperature;
            set
            {
                if (_heater.Temperature != value)
                {
                    _heater.Temperature = value;
                    OnPropertyChange(nameof(Temperature));
                }
            }
        }

        public bool IsOn => _heater.IsOn;

        public ICommand TurnOffCommand => _heater.TurnOffCommand;

        public ICommand TurnOnCommand => _heater.TurnOnCommand;

        public Heater(Model.IHeater heater)
        {
            _heater = heater;
            _heater.PositionChanged += GetPositionChanged;
            _heater.TemperatureChanged += GetTemperatureChanged;
            _heater.EnableChange += GetEnableChanged;
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

        private void GetEnableChanged(object? source, Model.EnableChangeEventArgs args)
        {
            // TODO: change to enableChanged
            EnableChange?.Invoke(this, new EnableChangeEventArgs(args.LastEnable, args.NewEnable));
            OnPropertyChange(nameof(IsOn));
            OnPropertyChange(nameof(Temperature));
        }

        public void Dispose()
        {
            _heater.PositionChanged -= GetPositionChanged;
            _heater.TemperatureChanged -= GetTemperatureChanged;
            _heater.EnableChange -= GetEnableChanged;
            _heater.Dispose();
            GC.SuppressFinalize(this);
        }

        protected void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
