using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TPUM.Data;

namespace TPUM.Presentation.Model
{
    internal class ModelHeatSensor : IModelHeatSensor
    {
        private readonly IHeatSensor _sensor;

        public long Id => _sensor.Id;
        public IPosition Position
        {
            get => _sensor.Position;
            set => _sensor.Position = value;
        }
        public float Temperature => _sensor.Temperature;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ModelHeatSensor(IHeatSensor sensor)
        {
            _sensor = sensor;
            _sensor.PositionChanged += GetPositionChanged;
            _sensor.TemperatureChanged += GetTemperatureChanged;
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

        public void Dispose()
        {
            _sensor.PositionChanged += GetPositionChanged;
            _sensor.TemperatureChanged += GetTemperatureChanged;
            _sensor.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}