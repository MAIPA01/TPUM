using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model.Tests
{
    internal class DummyHeatSensor : IHeatSensorModel
    {
        private readonly IHeatSensorLogic _logic;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id => _logic.Id;

        private IPositionModel _position;
        public IPositionModel Position
        {
            get => _position;
            set
            {
                _position.X = value.X;
                _position.Y = value.Y;
            }
        }

        private float _temperature;
        public float Temperature
        {
            get => _temperature;
            private set
            {
                float lastTemperature = _temperature;
                _temperature = value;
                OnTemperatureChanged(this, lastTemperature, _temperature);
            }
        }

        public DummyHeatSensor(IHeatSensorLogic logic)
        {
            _logic = logic;
            _position = new DummyPosition(_logic.Position);
            _position.PositionChanged += GetPositionChanged;
            Temperature = _logic.Temperature;
        }

        internal void UpdateData(float x, float y, float temperature)
        {
            _position.X = x;
            _position.Y = y;
            Temperature = temperature;
        }

        private void GetPositionChanged(object? source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(args.LastX, args.LastY, Position.X, Position.Y));
        }

        private void OnTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(lastTemperature, newTemperature));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}