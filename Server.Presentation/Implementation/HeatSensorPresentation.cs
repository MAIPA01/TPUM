using TPUM.Server.Logic;
using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    internal class HeatSensorPresentation : IHeatSensorPresentation
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly IHeatSensorLogic _logic;

        public Guid Id => _logic.Id;

        public IPositionPresentation Position
        {
            get => new PositionPresentation(_logic.Position);
            set => _logic.Position.SetPosition(value.X, value.Y);
        }

        public float Temperature => _logic.Temperature;

        public HeatSensorPresentation(IHeatSensorLogic logic)
        {
            _logic = logic;
            _logic.PositionChanged += GetPositionChanged;
            _logic.TemperatureChanged += GetTemperatureChanged;
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
        {
            PositionChanged?.Invoke(Guid.Empty, this, new PositionPresentation(lastPosition), new PositionPresentation(newPosition));
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(Guid.Empty, this, lastTemperature, newTemperature);
        }

        public void Dispose()
        {
            _logic.PositionChanged -= GetPositionChanged;
            _logic.TemperatureChanged -= GetTemperatureChanged;
            GC.SuppressFinalize(this);
        }
    }
}
