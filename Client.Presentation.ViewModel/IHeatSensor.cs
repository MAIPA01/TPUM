using TPUM.Client.Presentation.ViewModel.Events;
using System.ComponentModel;

namespace TPUM.Client.Presentation.ViewModel
{
    public interface IHeatSensor : INotifyPositionChanged, INotifyTemperatureChanged, INotifyPropertyChanged, IDisposable
    {
        Guid Id { get; }
        IPosition Position { get; }
        float Temperature { get; }

        void SetPosition(float x, float y);
    }
}