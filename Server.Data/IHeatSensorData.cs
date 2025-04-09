using TPUM.Server.Data.Events;

namespace TPUM.Server.Data
{
    public interface IHeatSensorData : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPositionData Position { get; set; }
        float Temperature { get; set; }
    }
}