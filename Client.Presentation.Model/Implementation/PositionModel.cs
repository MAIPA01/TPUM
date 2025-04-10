using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    internal class PositionModel : IPositionModel
    {
        public event PositionChangedEventHandler? PositionChanged;

        private readonly IPositionLogic _logic;

        public float X
        {
            get => _logic.X;
            set => _logic.X = value;
        }

        public float Y
        {
            get => _logic.Y;
            set => _logic.Y = value;
        }

        public PositionModel(IPositionLogic logic)
        {
            _logic = logic;
            _logic.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
        {
            PositionChanged?.Invoke(this, new PositionModel(lastPosition), new PositionModel(newPosition));
        }

        public void SetPosition(float x, float y)
        {
            _logic.SetPosition(x, y);
        }

        public override bool Equals(object? obj)
        {
            return obj switch
            {
                IPositionLogic logic => logic.Equals(_logic),
                IPositionModel model => Math.Abs(_logic.X - model.X) < 1e-10f && Math.Abs(_logic.Y - model.Y) < 1e-10f,
                _ => false
            };
        }

        public override int GetHashCode()
        {
            return _logic.GetHashCode();
        }

        public void Dispose()
        {
            _logic.PositionChanged -= GetPositionChanged;
            GC.SuppressFinalize(this);
        }
    }
}