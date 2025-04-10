using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    public interface IHeaterModel : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPositionModel Position { get; set; }
        float Temperature { get; set; }
        bool IsOn { get; }

        void TurnOn();
        void TurnOff();
    }
}