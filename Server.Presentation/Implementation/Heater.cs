using TPUM.Server.Logic;
using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    internal class Heater : IHeater
    {
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly IHeaterLogic _logic;
        public Guid Id => _logic.Id;
        public bool IsOn => _logic.IsOn;

        private readonly Position _position;
        public IPosition Position => _position;

        public float Temperature
        {
            get => _logic.Temperature;
            set => _logic.Temperature = value;
        }

        public Heater(IHeaterLogic logic)
        {
            _logic = logic;
            _logic.PositionChanged += GetPositionChanged;
            _logic.EnableChanged += GetEnableChanged;
            _logic.TemperatureChanged += GetTemperatureChanged;

            _position = new Position(_logic.Position);
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
        {
            PositionChanged?.Invoke(Guid.Empty, this, new Position(lastPosition), new Position(newPosition));
        }

        private void GetEnableChanged(object? source, bool lastEnabled, bool newEnabled)
        {
            EnableChanged?.Invoke(Guid.Empty, this, lastEnabled, newEnabled);
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(Guid.Empty, this, lastTemperature, newTemperature);
        }

        public void SetPosition(float x, float y)
        {
            _logic.SetPosition(x, y);
        }

        public void TurnOn()
        {
            _logic.TurnOn();
        }

        public void TurnOff()
        {
            _logic.TurnOff();
        }

        public void Dispose()
        {
            _logic.PositionChanged -= GetPositionChanged;
            _logic.TemperatureChanged -= GetTemperatureChanged;
            _logic.EnableChanged -= GetEnableChanged;
            GC.SuppressFinalize(this);
        }
    }
}