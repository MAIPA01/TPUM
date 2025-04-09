using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    public interface IHeaterPresentation : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        bool IsOn { get; }
        IPositionPresentation Position { get; set; }
        float Temperature { get; set; }
        void TurnOn();
        void TurnOff();
    }
}
