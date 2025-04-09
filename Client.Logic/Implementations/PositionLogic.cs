using TPUM.Client.Data;

namespace TPUM.Client.Logic
{
    internal class PositionLogic : IPositionLogic
    {
        private readonly IPositionData _data;

        private readonly object _xLock = new();
        public float X
        {
            get => _data.X;
            set
            {
                lock (_xLock)
                {
                    if (Math.Abs(_data.X - value) < 1e-10f) return;

                    float lastX = X;
                    float lastY = Y;
                    _data.X = value;
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

                    float lastX = X;
                    float lastY = Y;
                    _data.Y = value;
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
    }
}