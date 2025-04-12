using TPUM.Server.Data.Events;

namespace TPUM.Server.Data
{
    public interface IHeatSensorData : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPositionData Position { get; }
        float Temperature { get; set; }

        void SetPosition(float x, float y);
    }
}