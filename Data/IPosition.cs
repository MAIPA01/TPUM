using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Data
{
    public interface IPosition : INotifyPositionChanged, IDisposable
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}
