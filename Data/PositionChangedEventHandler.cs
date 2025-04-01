using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Data
{
    public delegate void PositionChangedEventHandler(object source, PositionChangedEventArgs e); 
    public class PositionChangedEventArgs(Position lastPosition, Position newPosition) : EventArgs
    {
        public Position LastPosition { get; private set; } = lastPosition;
        public Position NewPosition { get; private set; } = newPosition;
    }
}
