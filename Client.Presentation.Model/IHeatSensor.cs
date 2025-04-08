using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    public interface IHeatSensor : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPosition Position { get; set; }
        float Temperature { get; }
    }
}