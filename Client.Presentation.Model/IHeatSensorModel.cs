using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    public interface IHeatSensorModel : INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPositionModel Position { get; }
        float Temperature { get; }

        void SetPosition(float x, float y);
    }
}