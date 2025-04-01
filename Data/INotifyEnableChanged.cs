using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Data
{
    public interface INotifyEnableChanged
    {
        event EnableChangeEventHandler? EnableChange;
    }
}
