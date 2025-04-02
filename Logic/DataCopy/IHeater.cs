using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Logic
{
    public interface IHeater : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        long Id { get; }
        bool IsOn { get; }
        IPosition Position { get; set; }
        float Temperature { get; set; }
        void TurnOn();
        void TurnOff();
    }
}
