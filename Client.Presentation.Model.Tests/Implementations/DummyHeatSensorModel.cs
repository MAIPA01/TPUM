using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model.Tests
{
    internal class DummyHeatSensorModel : IHeatSensorModel
    {
        private readonly IHeatSensorLogic _logic;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id => _logic.Id;

        private readonly DummyPositionModel _position;
        public IPositionModel Position => _position;

        public float Temperature { get; }

        public DummyHeatSensorModel(IHeatSensorLogic logic)
        {
            _logic = logic;
            _position = new DummyPositionModel(_logic.Position);
            Temperature = _logic.Temperature;
        }

        public void SetPosition(float x, float y)
        {
            _position.SetPosition(x, y);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}