using TPUM.Server.Logic;
using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    internal class HeatSensor : IHeatSensor
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly IHeatSensorLogic _logic;

        public Guid Id => _logic.Id;

        private readonly Position _position;
        public IPosition Position => _position;

        public float Temperature => _logic.Temperature;

        public HeatSensor(IHeatSensorLogic logic)
        {
            _logic = logic;
            _logic.PositionChanged += GetPositionChanged;
            _logic.TemperatureChanged += GetTemperatureChanged;

            _position = new Position(_logic.Position);
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
        {
            PositionChanged?.Invoke(Guid.Empty, this, new Position(lastPosition), new Position(newPosition));
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(Guid.Empty, this, lastTemperature, newTemperature);
        }

        public void SetPosition(float x, float y)
        {
            _logic.SetPosition(x, y);
        }

        public void Dispose()
        {
            _logic.PositionChanged -= GetPositionChanged;
            _logic.TemperatureChanged -= GetTemperatureChanged;
            GC.SuppressFinalize(this);
        }
    }
}