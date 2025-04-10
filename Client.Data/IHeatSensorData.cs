using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    public interface IHeatSensorData : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPositionData Position { get; set; }
        float Temperature { get; }
    }
}