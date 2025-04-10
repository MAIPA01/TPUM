using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    internal class PositionData : IPositionData
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
                    if (MathF.Abs(_x - value) < 1e-10f) return;
                    var lastPosition = new PositionData(_x, _y);
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
                    if (MathF.Abs(_y - value) < 1e-10f) return;
                    var lastPosition = new PositionData(_x, _y);
                    _y = value;
                    PositionChanged?.Invoke(this, lastPosition, this);
                }
            }
        }

        public PositionData(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public void SetPosition(float x, float y)
        {
            lock (_posLock)
            {
                if (MathF.Abs(_x - x) < 1e-10f && MathF.Abs(_y - y) < 1e-10f) return;
                var lastPosition = new PositionData(_x, _y);
                _x = x;
                _y = y;
                PositionChanged?.Invoke(this, lastPosition, this);
            }
        }
    }
}