namespace TPUM.Data.Tests
{
    internal class DummyPosition(float x, float y) : IPosition
    {
        private float _x = x;
        public float X
        {
            get => _x;
            set
            {
                if (_x == value) return;
                lock (_xLock)
                {
                    DummyPosition lastPosition = new(_x, _y);
                    _x = value;
                    OnPositionChanged(this, lastPosition);
                }
            }
        }

        private float _y = y;
        public float Y
        {
            get => _y;
            set
            {

                if (_y == value) return;
                lock (_yLock)
                {
                    DummyPosition lastPosition = new(_x, _y);
                    _y = value;
                    OnPositionChanged(this, lastPosition);
                }
            }
        }

        public event PositionChangedEventHandler? PositionChanged;

        private readonly object _xLock = new();
        private readonly object _yLock = new();

        public static bool operator ==(DummyPosition pos1, IPosition pos2)
        {
            return Math.Abs(pos1.X - pos2.X) < 1e-10f && Math.Abs(pos1.Y - pos2.Y) < 1e-10f;
        }

        public static bool operator !=(DummyPosition pos1, IPosition pos2)
        {
            return Math.Abs(pos1.X - pos2.X) > 1e-10f || Math.Abs(pos1.Y - pos2.Y) > 1e-10f;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not IPosition position) return false;
            return this == position;
        }

        public override int GetHashCode()
        {
            return 3 * X.GetHashCode() + 5 * Y.GetHashCode();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void OnPositionChanged(object? source, DummyPosition lastPosition)
        {
            PositionChanged?.Invoke(source, new PositionChangedEventArgs(lastPosition, this));
        }
    }
}