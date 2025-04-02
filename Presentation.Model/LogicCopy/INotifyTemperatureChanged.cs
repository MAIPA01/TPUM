using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Presentation.Model
{
    public delegate void TemperatureChangedEventHandler(object? source, TemperatureChangedEventArgs e);

    public class TemperatureChangedEventArgs(float lastTemperature, float newTemperature) : EventArgs
    {
        public float LastTemperature { get; private set; } = lastTemperature;
        public float NewTemperature { get; private set; } = newTemperature;
    }

    public interface INotifyTemperatureChanged
    {
        event TemperatureChangedEventHandler? TemperatureChanged;
    }
}
