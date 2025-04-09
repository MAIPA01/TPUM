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
            set
            {
                var lastPosition = new PositionPresentation(_logic.Position);
                _logic.PositionChanged -= GetPositionChanged;
                _logic.Position.X = value.X;
                _logic.Position.Y = value.Y;
                _logic.PositionChanged += GetPositionChanged;
                OnPositionChanged(lastPosition);
            }
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
            PositionChanged?.Invoke(this, new PositionPresentation(lastPosition), new PositionPresentation(newPosition));
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, lastTemperature, newTemperature);
        }

        private void OnPositionChanged(IPositionPresentation lastPosition)
        {
            PositionChanged?.Invoke(this, lastPosition, Position);
        }

        public void Dispose()
        {
            _logic.PositionChanged -= GetPositionChanged;
            _logic.TemperatureChanged -= GetTemperatureChanged;
            GC.SuppressFinalize(this);
        }
    }
}
