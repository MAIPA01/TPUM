using TPUM.Client.Presentation.ViewModel.Events;
using System.ComponentModel;

namespace TPUM.Client.Presentation.ViewModel
{
    public interface IHeater : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, INotifyPropertyChanged, IDisposable
    {
        Guid Id { get; }
        IPosition Position { get; set; }
        float CurrentTemperature { get; }
        float DesiredTemperature { get; set; }
        bool IsOn { get; }

        string TurnText { get; }
        ICommand TurnCommand { get; }
    }
}