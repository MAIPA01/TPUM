using TPUM.Client.Presentation.ViewModel.Events;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TPUM.Client.Presentation.Model;

namespace TPUM.Client.Presentation.ViewModel
{
    internal class HeatSensor : IHeatSensor
    {
        private readonly IHeatSensorModel _model;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Guid Id => _model.Id;

        private readonly Position _position;
        public IPosition Position => _position;

        public float Temperature => _model.Temperature;

        public HeatSensor(IHeatSensorModel model)
        {
            _model = model;
            _model.PositionChanged += GetPositionChanged;
            _model.TemperatureChanged += GetTemperatureChanged;

            _position = new Position(_model.Position);
        }

        private void GetPositionChanged(object? source, IPositionModel lastPosition, IPositionModel newPosition)
        {
            PositionChanged?.Invoke(this, new Position(lastPosition), new Position(newPosition));
            OnPropertyChange(nameof(Position));
            OnPropertyChange(nameof(Temperature));
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, lastTemperature, newTemperature);
            OnPropertyChange(nameof(Temperature));
        }

        public void SetPosition(float x, float y)
        {
            _model.SetPosition(x, y);
        }

        public void Dispose()
        {
            _model.PositionChanged -= GetPositionChanged;
            _model.TemperatureChanged -= GetTemperatureChanged;
            GC.SuppressFinalize(this);
        }

        protected void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}