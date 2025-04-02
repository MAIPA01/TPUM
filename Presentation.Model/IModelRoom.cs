using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TPUM.Logic;

namespace TPUM.Presentation.Model
{
    public interface IModelRoom : INotifyTemperatureChanged, INotifyPositionChanged, INotifyEnableChanged, INotifyPropertyChanged, IDisposable
    {
        long Id { get; }
        string Name { get; set; }
        float Width { get; }
        float Height { get; }
        float AvgTemperature { get; }

        ReadOnlyObservableCollection<IModelHeater> Heaters { get; }
        ReadOnlyObservableCollection<IModelHeatSensor> HeatSensors { get; }

        ICommand ClearHeatSensorsCommand { get; }
        ICommand ClearHeatersCommand { get; }

        void AddHeater(float x, float y, float temperature);
        void RemoveHeater(long id);
        void AddHeatSensor(float x, float y);
        void RemoveHeatSensor(long id);
    }
}
