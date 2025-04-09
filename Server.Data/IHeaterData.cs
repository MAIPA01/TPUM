using TPUM.Server.Data.Events;

namespace TPUM.Server.Data
{
    public interface IHeaterData : INotifyPositionChanged, INotifyEnableChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        bool IsOn { get; set; }
        IPositionData Position { get; set; }
        float Temperature { get; set; }
    }
}