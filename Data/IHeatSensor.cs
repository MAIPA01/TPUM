namespace TPUM.Data
{
    public interface IHeatSensor : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        long Id { get; }
        Position Position { get; set; }
        float Temperature { get; set; }
    }
}
