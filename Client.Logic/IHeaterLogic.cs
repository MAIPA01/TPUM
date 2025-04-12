using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    public interface IHeaterLogic : INotifyPositionChanged, INotifyTemperatureChanged, INotifyEnableChanged, IDisposable
    {
        Guid Id { get; }
        bool IsOn { get; }
        IPositionLogic Position { get; }
        float Temperature { get; set; }

        void SetPosition(float x, float y);

        void TurnOn();
        void TurnOff();
    }
}