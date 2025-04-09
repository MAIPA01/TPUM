namespace TPUM.Server.Data
{
    internal class HeatSensorData : IHeatSensorData
    {
        public Guid Id { get; }
        public IPositionData Position { get; set; }
        public float Temperature { get; set; } = 0.0f;

        public HeatSensorData(Guid id, IPositionData position)
        {
            Id = id;
            Position = position;
        }
    }
}