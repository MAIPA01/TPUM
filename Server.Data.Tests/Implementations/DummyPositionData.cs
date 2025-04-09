using TPUM.Server.Data.Events;

namespace TPUM.Server.Data.Tests
{
    internal class DummyPositionData : IPositionData
    {
        public event PositionChangedEventHandler? PositionChanged;

        private readonly object _posLock = new();

        private float _x;
        public float X
        {
            get
            {
                lock (_posLock)
                {
                    return _x;
                }
            }
            set
            {
                lock (_posLock)
                {
                    if (Math.Abs(_x - value) < 1e-10f) return;
                    var lastPosition = new DummyPositionData(_x, _y);
                    _x = value;
                    PositionChanged?.Invoke(this, lastPosition, this);
                }
            }
        }

        private float _y;
        public float Y
        {
            get
            {
                lock (_posLock)
                {
                    return _y;
                }
            }
            set
            {
                lock (_posLock)
                {
                    if (Math.Abs(_y - value) < 1e-10f) return;
                    var lastPosition = new DummyPositionData(_x, _y);
                    _y = value;
                    PositionChanged?.Invoke(this, lastPosition, this);
                }
            }
        }

        public DummyPositionData(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public void SetPosition(float x, float y)
        {
            lock (_posLock)
            {
                if (Math.Abs(_x - x) < 1e-10f && Math.Abs(_y - y) < 1e-10f) return;
                var lastPosition = new DummyPositionData(_x, _y);
                _x = x;
                _y = y;
                PositionChanged?.Invoke(this, lastPosition, this);
            }
        }

        public override bool Equals(object? obj)
        {
            lock (_posLock)
            {
                if (obj is not IPositionData position) return false;
                return Math.Abs(X - position.X) < 1e-10f && Math.Abs(Y - position.Y) < 1e-10f;
            }
        }

        public override int GetHashCode()
        {
            lock (_posLock)
            {
                return 3 * X.GetHashCode() + 5 * Y.GetHashCode();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}