using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Logic
{
    internal class Position : IPosition
    {
        private readonly Data.IPosition _position;

        public event PositionChangedEventHandler? PositionChanged;

        public float X
        {
            get => _position.X;
            set => _position.X = value;
        }

        public float Y
        {
            get => _position.Y;
            set => _position.Y = value;
        }

        public Position(Data.IPosition position)
        {
            _position = position;
            _position.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object source, Data.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), this));
        }

        public void Dispose()
        {
            _position.PositionChanged -= GetPositionChanged;
            _position.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
