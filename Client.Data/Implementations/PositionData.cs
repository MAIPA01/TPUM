namespace TPUM.Client.Data
{
    internal class PositionData : IPositionData
    {
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
        }

        public PositionData(float x, float y)
        {
            _x = x;
            _y = y;
        }

        internal void SetPosition(float x, float y)
        {
            lock (_posLock)
            {
                if (MathF.Abs(_x - x) < 1e-10f && MathF.Abs(_y - y) < 1e-10f) return;
                _x = x;
                _y = y;
            }
        }

        public override bool Equals(object? obj)
        {
            lock (_posLock)
            {
                if (obj is not IPositionData position) return false;
                return Math.Abs(_x - position.X) < 1e-10f && Math.Abs(_y - position.Y) < 1e-10f;
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