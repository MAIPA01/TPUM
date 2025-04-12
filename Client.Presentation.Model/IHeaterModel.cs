using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    public interface IHeaterModel : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        IPositionModel Position { get; }
        float Temperature { get; set; }
        bool IsOn { get; }

        void SetPosition(float x, float y);
        void TurnOn();
        void TurnOff();
    }
}