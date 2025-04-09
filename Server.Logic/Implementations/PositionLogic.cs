using TPUM.Server.Data;
using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
{
    internal class PositionLogic : IPositionLogic
    {
        private readonly IPositionData _data;

        public event PositionChangedEventHandler? PositionChanged;

        private readonly object _xLock = new();
        public float X
        {
            get {
                lock (_xLock)
                {
                    return _data.X;
                }
            }
            set
            {
                lock (_xLock)
                {
                    if (Math.Abs(_data.X - value) < 1e-10f) return;

                    PositionLogic last = new(DataApiBase.GetApi().CreatePosition(_data.X, Y));
                    _data.X = value;
                    OnPositionChanged(this, last);
                }
            }
        }

        private readonly object _yLock = new();
        public float Y
        {
            get
            {
                lock (_yLock)
                {
                    return _data.Y;
                }
            }
            set
            {
                lock (_yLock)
                {
                    if (Math.Abs(_data.Y - value) < 1e-10f) return;

                    PositionLogic last = new(DataApiBase.GetApi().CreatePosition(X, _data.Y));
                    _data.Y = value;
                    OnPositionChanged(this, last);
                }
            }
        }

        public PositionLogic(IPositionData data)
        {
            _data = data;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not IPositionLogic position) return false;
            return Math.Abs(X - position.X) < 1e-10f && Math.Abs(Y - position.Y) < 1e-10f;
        }

        public override int GetHashCode()
        {
            return 3 * _data.X.GetHashCode() + 5 * _data.Y.GetHashCode();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void OnPositionChanged(object? source, IPositionLogic lastPosition)
        {
            PositionChanged?.Invoke(source, lastPosition, this);
        }
    }
}