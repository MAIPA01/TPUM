using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    public interface IHeater : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        bool IsOn { get; }
        IPosition Position { get; }
        float Temperature { get; set; }

        void SetPosition(float x, float y);
        void TurnOn();
        void TurnOff();
    }
}
