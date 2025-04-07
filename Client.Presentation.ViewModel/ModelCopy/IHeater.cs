using System.ComponentModel;

namespace TPUM.Client.Presentation.ViewModel
{
    public interface IHeater : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, INotifyPropertyChanged, IDisposable
    {
        long Id { get; }
        IPosition Position { get; set; }
        float Temperature { get; set; }
        bool IsOn { get; }

        string TurnText { get; }
        ICommand TurnCommand { get; }
    }
}