using System.ComponentModel;
using System.Windows.Input;
using TPUM.Data;

namespace TPUM.Presentation.Model
{
    public interface IModelHeater : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, INotifyPropertyChanged, IDisposable
    {
        long Id { get; }
        IPosition Position { get; set; }
        float Temperature { get; set; }
        bool IsOn { get; }

        void TurnOn();
        void TurnOff();
    }
}
