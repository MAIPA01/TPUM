using TPUM.Client.Presentation.ViewModel.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TPUM.Client.Presentation.ViewModel
{
    public interface IRoom : INotifyHeaterAdded, INotifyHeaterRemoved, INotifyHeatSensorAdded, INotifyHeatSensorRemoved,
        INotifyTemperatureChanged, INotifyPositionChanged, INotifyEnableChanged, INotifyPropertyChanged, IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }
        float AvgTemperature { get; }

        ReadOnlyObservableCollection<IHeater> Heaters { get; }
        ReadOnlyObservableCollection<IHeatSensor> HeatSensors { get; }

        void AddHeater(float x, float y, float temperature);
        void RemoveHeater(Guid id);
        void AddHeatSensor(float x, float y);
        void RemoveHeatSensor(Guid id);
    }
}