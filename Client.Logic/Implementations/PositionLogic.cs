using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    internal class PositionLogic : IPositionLogic
    {
        public event PositionChangedEventHandler? PositionChanged;

        private readonly IPositionData _data;

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
            _data.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object? source, IPositionData lastPosition, IPositionData newPosition)
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
            _data.PositionChanged -= GetPositionChanged;
            GC.SuppressFinalize(this);
        }
    }
}