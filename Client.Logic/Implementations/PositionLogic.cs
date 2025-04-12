using TPUM.Client.Data;

namespace TPUM.Client.Logic
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
                IPositionData data => data.Equals(_data),
                IPositionLogic logic => Math.Abs(_data.X - logic.X) < 1e-10f && Math.Abs(_data.Y - logic.Y) < 1e-10f,
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