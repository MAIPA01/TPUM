using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Data
{
    public delegate void EnableChangeEventHandler(object source, EnableChangeEventArgs e);
    public class EnableChangeEventArgs(bool lastEnable, bool newEnable) : EventArgs
    {
        public bool LastEnable { get; private set; } = lastEnable;
        public bool NewEnable { get; private set; } = newEnable;
    }

    public interface INotifyEnableChanged
    {
        event EnableChangeEventHandler? EnableChange;
    }
}
