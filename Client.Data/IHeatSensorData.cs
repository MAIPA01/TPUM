using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    public interface IHeatSensorData : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPositionData Position { get; }
        float Temperature { get; }

        void SetPosition(float x, float y);
    }
}