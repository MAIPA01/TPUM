using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model.Tests
{
    internal class DummyHeater : IHeaterModel
    {
        private readonly IHeaterLogic _logic;

        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id => _logic.Id;
        private readonly DummyPositionModel _position;
        public IPositionModel Position => _position;

        public float Temperature { get; set; }

        public bool IsOn { get; private set; }

        public DummyHeater(IHeaterLogic logic)
        {
            _logic = logic;
            _position = new DummyPositionModel(_logic.Position);
            Temperature = _logic.Temperature;
        }

        public void SetPosition(float x, float y)
        {
            _position.SetPosition(x, y);
        }

        public void TurnOn()
        {
            if (IsOn) return;
            IsOn = true;
        }

        public void TurnOff()
        {
            if (!IsOn) return;
            IsOn = false;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}