using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic.Tests
{
    internal class DummyHeaterLogic : IHeaterLogic
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;

        private readonly IHeaterData _data;

        public Guid Id => _data.Id;

        public bool IsOn => _data.IsOn;

        private readonly DummyPositionLogic _position;
        public IPositionLogic Position => _position;

        public float Temperature
        {
            get => _data.Temperature;
            set => _data.Temperature = value;
        }

        public DummyHeaterLogic(IHeaterData data)
        {
            _data = data;
            _position = new DummyPositionLogic(_data.Position);
        }

        public void SetPosition(float x, float y)
        {
            _data.SetPosition(x, y);
        }

        public void TurnOn()
        {
            _data.IsOn = true;
        }

        public void TurnOff()
        {
            _data.IsOn = false;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}