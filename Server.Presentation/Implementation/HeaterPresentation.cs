using TPUM.Server.Logic;
using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    internal class HeaterPresentation : IHeaterPresentation
    {
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly IHeaterLogic _logic;
        public Guid Id => _logic.Id;
        public bool IsOn => _logic.IsOn;

        public IPositionPresentation Position
        {
            get => new PositionPresentation(_logic.Position);
            set
            {
                var lastPosition = new PositionPresentation(_logic.Position);
                _logic.PositionChanged -= GetPositionChanged;
                _logic.Position.X = value.X;
                _logic.Position.Y = value.Y;
                _logic.PositionChanged += GetPositionChanged;
                OnPositionChange(lastPosition);
            }
        }

        public float Temperature
        {
            get => _logic.Temperature;
            set => _logic.Temperature = value;
        }

        public HeaterPresentation(IHeaterLogic logic)
        {
            _logic = logic;
            _logic.PositionChanged += GetPositionChanged;
            _logic.EnableChanged += GetEnableChanged;
            _logic.TemperatureChanged += GetTemperatureChanged;
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
        {
            PositionChanged?.Invoke(this, new PositionPresentation(lastPosition), new PositionPresentation(newPosition));
        }

        private void GetEnableChanged(object? source, bool lastEnabled, bool newEnabled)
        {
            EnableChanged?.Invoke(this, lastEnabled, newEnabled);
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, lastTemperature, newTemperature);
        }

        public void TurnOn()
        {
            _logic.TurnOn();
        }

        public void TurnOff()
        {
            _logic.TurnOff();
        }

        private void OnPositionChange(IPositionPresentation lastPosition)
        {
            PositionChanged?.Invoke(this, lastPosition, Position);
        }

        public void Dispose()
        {
            _logic.PositionChanged -= GetPositionChanged;
            _logic.TemperatureChanged -= GetTemperatureChanged;
            _logic.EnableChanged -= GetEnableChanged;
        }
    }
}
