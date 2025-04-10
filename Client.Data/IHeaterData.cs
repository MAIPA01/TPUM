using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    public interface IHeaterData : INotifyPositionChanged, INotifyEnableChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        bool IsOn { get; set; }
        IPositionData Position { get; set; }
        float Temperature { get; set; }
    }
}