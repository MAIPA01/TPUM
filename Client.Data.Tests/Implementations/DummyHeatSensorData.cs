namespace TPUM.Client.Data.Tests
{
    internal class DummyHeatSensorData : IHeatSensorData
    {
        public Guid Id { get; }
        public IPositionData Position { get; set; }
        public float Temperature { get; set; } = 0.0f;

        public DummyHeatSensorData(Guid id, IPositionData position)
        {
            Id = id;
            Position = position;
        }
    }
}