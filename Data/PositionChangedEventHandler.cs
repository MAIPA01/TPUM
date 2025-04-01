using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Data
{
    public delegate void PositionChangedEventHandler(object source, PositionChangedEventArgs e); 
    public class PositionChangedEventArgs(IPosition lastPosition, IPosition newPosition) : EventArgs
    {
        public IPosition LastPosition { get; private set; } = lastPosition;
        public IPosition NewPosition { get; private set; } = newPosition;
    }
}
