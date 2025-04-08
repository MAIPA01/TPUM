using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TPUM.Client.Presentation.ViewModel
{
    public interface IRoom : INotifyTemperatureChanged, INotifyPositionChanged, INotifyEnableChanged, INotifyPropertyChanged, IDisposable
    {
        Guid Id { get; }
        string Name { get; set; }
        float Width { get; }
        float Height { get; }
        float AvgTemperature { get; }

        ReadOnlyObservableCollection<IHeater> Heaters { get; }
        ReadOnlyObservableCollection<IHeatSensor> HeatSensors { get; }

        ICommand ClearHeatSensorsCommand { get; }
        ICommand ClearHeatersCommand { get; }

        IHeater AddHeater(float x, float y, float temperature);
        void RemoveHeater(Guid id);
        IHeatSensor AddHeatSensor(float x, float y);
        void RemoveHeatSensor(Guid id);
    }
}