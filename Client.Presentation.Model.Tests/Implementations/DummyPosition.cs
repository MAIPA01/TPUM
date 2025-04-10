using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model.Tests
{
    internal class DummyPosition : IPositionModel
    {
        public event PositionChangedEventHandler? PositionChanged;

        private float _x;
        public float X
        {
            get => _x;
            set
            {
                if (MathF.Abs(_x - value) < 1e-10f) return;
                float lastX = _x;
                float lastY = _y;
                _x = value;
                OnPositionChanged(this, lastX, lastY);
            }
        }

        private float _y;
        public float Y
        {
            get => _y;
            set
            {
                if (MathF.Abs(_y - value) < 1e-10f) return;
                float lastX = _x;
                float lastY = _y;
                _y = value;
                OnPositionChanged(this, lastX, lastY);
            }
        }

        public DummyPosition(IPositionLogic logic)
        {
            X = logic.X;
            Y = logic.Y;
        }

        internal void UpdateData(float x, float y)
        {
            X = x;
            Y = y;
        }

        private void OnPositionChanged(object? source, float lastX, float lastY)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(lastX, lastY, X, Y));
        }

        public static bool operator ==(DummyPosition pos1, IPositionModel pos2)
        {
            return Math.Abs(pos1.X - pos2.X) < 1e-10f && Math.Abs(pos1.Y - pos2.Y) < 1e-10f;
        }

        public static bool operator !=(DummyPosition pos1, IPositionModel pos2)
        {
            return Math.Abs(pos1.X - pos2.X) > 1e-10f || Math.Abs(pos1.Y - pos2.Y) > 1e-10f;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not IPositionModel position) return false;
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
    }
}