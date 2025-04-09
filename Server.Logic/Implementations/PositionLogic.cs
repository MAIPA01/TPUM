using TPUM.Server.Data;
using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
{
    internal class PositionLogic : IPositionLogic
    {
        private readonly IPositionData _data;

        public event PositionChangedEventHandler? PositionChanged;

        public float X
        {
            get => _data.X;
            set => _data.X = value;
        }

        public float Y
        {
            get => _data.Y;
            set => _data.Y = value;
        }

        public PositionLogic(IPositionData data)
        {
            _data = data;
            _data.PositionChanged += GetPositionChange;
        }

        private void GetPositionChange(object? source, IPositionData lastPosition, IPositionData newPosition)
        {
            PositionChanged?.Invoke(this, new PositionLogic(lastPosition), new PositionLogic(newPosition));
        }

        public void SetPosition(float x, float y)
        {
            _data.SetPosition(x, y);
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
            _data.PositionChanged -= GetPositionChange;
            GC.SuppressFinalize(this);
        }
    }
}