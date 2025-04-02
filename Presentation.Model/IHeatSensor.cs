namespace TPUM.Presentation.Model
{
    public interface IHeatSensor : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        long Id { get; }
        IPosition Position { get; set; }
        float Temperature { get; }
    }
}