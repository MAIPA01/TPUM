using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Presentation.ViewModel
{
    public interface IPosition : INotifyPositionChanged, INotifyPropertyChanged, IDisposable
    {
        float X { get; set; }
        float Y { get; set; }

        static float Distance(IPosition pos1, IPosition pos2)
        {
            return MathF.Sqrt((pos1.X - pos2.X) * (pos1.X - pos2.X) + (pos1.Y - pos2.Y) * (pos1.Y - pos2.Y));
        }
    }
}
