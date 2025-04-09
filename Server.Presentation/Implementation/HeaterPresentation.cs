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
            set => _logic.Position.SetPosition(value.X, value.Y);
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
            PositionChanged?.Invoke(Guid.Empty, this, new PositionPresentation(lastPosition), new PositionPresentation(newPosition));
        }

        private void GetEnableChanged(object? source, bool lastEnabled, bool newEnabled)
        {
            EnableChanged?.Invoke(Guid.Empty, this, lastEnabled, newEnabled);
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(Guid.Empty, this, lastTemperature, newTemperature);
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
