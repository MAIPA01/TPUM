using TPUM.Client.Data.Events;

namespace TPUM.Client.Data.Tests
{
    internal class DummyHeatSensorData : IHeatSensorData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id { get; }
        private readonly DummyPositionData _position;
        public IPositionData Position => _position;
        public float Temperature { get; set; }

        public DummyHeatSensorData(Guid id, float x, float y, float temperature = 0.0f)
        {
            Id = id;
            _position = new DummyPositionData(x, y);
            Temperature = temperature;
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