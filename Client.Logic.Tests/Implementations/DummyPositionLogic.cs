using TPUM.Client.Data;

namespace TPUM.Client.Logic.Tests
{
    internal class DummyPositionLogic : IPositionLogic
    {
        private readonly IPositionData _data;

        private readonly object _posLock = new();

        public float X => _data.X;
        public float Y => _data.Y;

        public DummyPositionLogic(IPositionData data)
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