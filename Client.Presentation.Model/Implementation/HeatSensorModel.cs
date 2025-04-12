using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    internal class HeatSensorModel : IHeatSensorModel
    {
        private readonly IHeatSensorLogic _logic;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id => _logic.Id;

        private readonly PositionModel _position;
        public IPositionModel Position => _position;

        public float Temperature => _logic.Temperature;

        public HeatSensorModel(IHeatSensorLogic logic)
        {
            _logic = logic;
            _logic.PositionChanged += GetPositionChanged;
            _logic.TemperatureChanged += GetTemperatureChanged;

            _position = new PositionModel(_logic.Position);
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
        {
            PositionChanged?.Invoke(this, new PositionModel(lastPosition), new PositionModel(newPosition));
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, lastTemperature, newTemperature);
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