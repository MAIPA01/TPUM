using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TPUM.Presentation.ViewModel
{
    public interface IRoom : INotifyTemperatureChanged, INotifyPositionChanged, INotifyEnableChanged, INotifyPropertyChanged, IDisposable
    {
        long Id { get; }
        string Name { get; set; }
        float Width { get; }
        float Height { get; }
        float AvgTemperature { get; }

        ReadOnlyObservableCollection<IHeater> Heaters { get; }
        ReadOnlyObservableCollection<IHeatSensor> HeatSensors { get; }

        ICommand ClearHeatSensorsCommand { get; }
        ICommand ClearHeatersCommand { get; }

        IHeater AddHeater(float x, float y, float temperature);
        void RemoveHeater(long id);
        IHeatSensor AddHeatSensor(float x, float y);
        void RemoveHeatSensor(long id);
    }
}
