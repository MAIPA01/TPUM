using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    public interface IHeatSensorLogic : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPositionLogic Position { get; set; }
        float Temperature { get; }
    }
}