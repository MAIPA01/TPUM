namespace TPUM.Client.Data.Tests
{
    internal class DummyHeatSensorData : IHeatSensorData
    {
        public Guid Id { get; }
        public IPositionData Position { get; set; }
        public float Temperature { get; set; }

        public DummyHeatSensorData(Guid id, IPositionData position, float temperature = 0.0f)
        {
            Id = id;
            Position = position;
            Temperature = temperature;
        }
    }
}