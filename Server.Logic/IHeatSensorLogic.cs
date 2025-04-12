using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
{
    public interface IHeatSensorLogic : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPositionLogic Position { get; }
        float Temperature { get; }

        void SetPosition(float x, float y);
    }
}