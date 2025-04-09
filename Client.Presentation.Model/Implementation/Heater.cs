using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    internal class Heater : IHeater
    {
        private readonly IHeaterLogic _logic;
        private readonly ModelApi _modelApi;

        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id => _logic.Id;
        private IPosition _position;
        public IPosition Position
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
            set
            {
                if (MathF.Abs(_temperature - value) < 1e-10f) return;
                float lastTemperature = _temperature;
                _temperature = value;
                OnTemperatureChanged(this, lastTemperature, _temperature);
            }
        }

        private bool _isOn;
        public bool IsOn
        {
            get => _isOn;
            private set
            {
                if (_isOn == value) return;
                bool lastValue = _isOn;
                _isOn = value;
                OnEnableChanged(this, lastValue, _isOn);
            }
        }

        public Heater(IHeaterLogic logic, ModelApi api)
        {
            _logic = logic;
            _modelApi = api;

            _position = new Position(_logic.Position);
            _position.PositionChanged += GetPositionChanged;
            Temperature = _logic.Temperature;
        }

        internal void UpdateData(float x, float y, float temperature, bool isOn)
        {
            _position.X = x;
            _position.Y = y;
            Temperature = temperature;
            IsOn = isOn;
        }

        private void GetPositionChanged(object? source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(args.LastX, args.LastY, Position.X, Position.Y));
        }

        private void OnTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(lastTemperature, newTemperature));
        }

        private void OnEnableChanged(object? source, bool lastValue, bool newValue)
        {
            EnableChanged?.Invoke(this, new EnableChangedEventArgs(lastValue, newValue));
        }

        public void TurnOn()
        {
            if (IsOn) return;
            _modelApi.UpdateHeater(Id, Position.X, Position.Y, Temperature, true);
        }

        public void TurnOff()
        {
            if (!IsOn) return;
            _modelApi.UpdateHeater(Id, Position.X, Position.Y, Temperature, false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}