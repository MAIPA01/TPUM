using TPUM.Client.Logic;

namespace TPUM.Client.Presentation.Model.Tests
{
    internal class DummyPositionModel : IPositionModel
    {
        private readonly IPositionLogic _logic;

        public float X { get; private set; }
        public float Y { get; private set; }

        public DummyPositionModel(IPositionLogic logic)
        {
            _logic = logic;
            X = _logic.X;
            Y = _logic.Y;
        }

        internal void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(DummyPositionModel pos1, IPositionModel pos2)
        {
            return Math.Abs(pos1.X - pos2.X) < 1e-10f && Math.Abs(pos1.Y - pos2.Y) < 1e-10f;
        }

        public static bool operator !=(DummyPositionModel pos1, IPositionModel pos2)
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