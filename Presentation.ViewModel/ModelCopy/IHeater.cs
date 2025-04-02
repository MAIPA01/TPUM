using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TPUM.Presentation.ViewModel
{
    public interface IHeater : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, INotifyPropertyChanged, IDisposable
    {
        long Id { get; }
        IPosition Position { get; set; }
        float Temperature { get; set; }
        bool IsOn { get; }

        ICommand TurnOffCommand { get; }
        ICommand TurnOnCommand { get; }

        //void TurnOn();
        //void TurnOff();
    }
}
