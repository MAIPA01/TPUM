using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    public interface IHeatSensorPresentation : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPositionPresentation Position { get; set; }
        float Temperature { get; }
    }
}
