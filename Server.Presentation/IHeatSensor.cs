using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    public interface IHeatSensor : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPosition Position { get; }
        float Temperature { get; }

        void SetPosition(float x, float y);
    }
}
