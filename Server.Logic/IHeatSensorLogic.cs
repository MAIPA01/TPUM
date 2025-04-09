using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
{
    public interface IHeatSensorLogic : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPositionLogic Position { get; set; }
        float Temperature { get; }
    }
}