using TPUM.Server.Data;

namespace TPUM.Server.Logic
{
    internal class PositionLogic : IPositionLogic
    {
        private readonly IPositionData _data;

        public float X => _data.X;

        public float Y => _data.Y;

        public PositionLogic(IPositionData data)
        {
            _data = data;
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