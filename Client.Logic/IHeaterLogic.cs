using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    public interface IHeaterLogic : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        bool IsOn { get; }
        IPositionLogic Position { get; set; }
        float Temperature { get; set; }
        void TurnOn();
        void TurnOff();
    }
}