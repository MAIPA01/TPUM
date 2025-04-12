using TPUM.Server.Data;

namespace TPUM.Server.Logic.Tests
{
    internal class DummyPositionLogic : IPositionLogic
    {
        private readonly IPositionData _data;

        public float X { get; private set; }

        public float Y { get; private set; }

        public DummyPositionLogic(IPositionData data)
        {
            _data = data;
            X = _data.X;
            Y = _data.Y;
        }

        public void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            return obj switch
            {
                IPositionLogic logic => Math.Abs(X - logic.X) < 1e-10f && Math.Abs(Y - logic.Y) < 1e-10f,
                IPositionData data => _data.Equals(data),
                _ => false
            };
        }

        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}