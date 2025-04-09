using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
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