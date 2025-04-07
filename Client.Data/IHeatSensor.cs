namespace TPUM.Client.Data
{
    public interface IHeatSensor : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        long Id { get; }
        IPosition Position { get; set; }
        float Temperature { get; set; }
    }
}