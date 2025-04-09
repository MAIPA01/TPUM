namespace TPUM.Server.Data
{
    public interface IHeatSensorData
    {
        Guid Id { get; }
        IPositionData Position { get; set; }
        float Temperature { get; set; }
    }
}