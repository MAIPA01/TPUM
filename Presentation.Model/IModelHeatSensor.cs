using System.ComponentModel;
using System.Windows.Input;
using TPUM.Data;

namespace TPUM.Presentation.Model
{
    public interface IModelHeatSensor : INotifyPositionChanged, INotifyTemperatureChanged, INotifyPropertyChanged, IDisposable
    {
        long Id { get; }
        IPosition Position { get; set; }
        float Temperature { get; }
    }
}
