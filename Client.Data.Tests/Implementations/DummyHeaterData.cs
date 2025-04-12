using TPUM.Client.Data.Events;

namespace TPUM.Client.Data.Tests
{
    internal class DummyHeaterData : IHeaterData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id { get; }
        public bool IsOn { get; set; }
        private readonly DummyPositionData _position;
        public IPositionData Position => _position;
        public float Temperature { get; set; }

        public DummyHeaterData(Guid id, float x, float y, float temperature, bool isOn = false)
        {
            Id = id;
            _position = new DummyPositionData(x, y);
            Temperature = temperature;
            IsOn = isOn;
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