using System.ComponentModel;

namespace TPUM.Presentation.ViewModel
{
    public interface IHeatSensor : INotifyPositionChanged, INotifyTemperatureChanged, INotifyPropertyChanged, IDisposable
    {
        long Id { get; }
        IPosition Position { get; set; }
        float Temperature { get; }
    }
}