using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic.Tests
{
    internal class DummyPositionLogic : IPositionLogic
    {
        private readonly IPositionData _data;

        public event PositionChangedEventHandler? PositionChanged;

        private readonly object _xLock = new();
        public float X
        {
            get => _data.X;
            set
            {
                lock (_xLock)
                {
                    if (Math.Abs(_data.X - value) < 1e-10f) return;

                    DummyPositionLogic last = new(DataApiBase.GetApi().CreatePosition(_data.X, _data.Y));
                    _data.X = value;
                    OnPositionChanged(this, last);
                }
            }
        }

        private readonly object _yLock = new();
        public float Y
        {
            get => _data.Y;
            set
            {
                lock (_yLock)
                {
                    if (Math.Abs(_data.Y - value) < 1e-10f) return;

                    DummyPositionLogic last = new(DataApiBase.GetApi().CreatePosition(_data.X, _data.Y));
                    _data.Y = value;
                    OnPositionChanged(this, last);
                }
            }
        }

        public DummyPositionLogic(IPositionData data)
        {
            _data = data;
        }

        public static bool operator ==(DummyPositionLogic pos1, IPositionLogic pos2)
        {
            return Math.Abs(pos1.X - pos2.X) < 1e-10f && Math.Abs(pos1.Y - pos2.Y) < 1e-10f;
        }

        public static bool operator !=(DummyPositionLogic pos1, IPositionLogic pos2)
        {
            return Math.Abs(pos1.X - pos2.X) > 1e-10f || Math.Abs(pos1.Y - pos2.Y) > 1e-10f;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not IPositionLogic position) return false;
            return this == position;
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
            PositionChanged?.Invoke(source, new PositionChangedEventArgs(lastPosition, this));
        }
    }
}