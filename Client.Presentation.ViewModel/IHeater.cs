using System.ComponentModel;
using TPUM.Client.Presentation.ViewModel.Events;

namespace TPUM.Client.Presentation.ViewModel
{
    public interface IHeater : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, INotifyPropertyChanged, IDisposable
    {
        Guid Id { get; }
        IPosition Position { get; }
        float CurrentTemperature { get; }
        float DesiredTemperature { get; set; }
        bool IsOn { get; }

        string TurnText { get; }
        ICommand TurnCommand { get; }

        void SetPosition(float x, float y);
    }
}