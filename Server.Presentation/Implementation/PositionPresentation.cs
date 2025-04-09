using TPUM.Server.Logic;
using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    internal class PositionPresentation : IPositionPresentation
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

        public PositionPresentation(IPositionLogic logic)
        {
            _logic = logic;
            _logic.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
        {
            PositionChanged?.Invoke(Guid.Empty, this, new PositionPresentation(lastPosition), new PositionPresentation(newPosition));
        }

        public void SetPosition(float x, float y)
        {
            _logic.SetPosition(x, y);
        }

        public void Dispose()
        {
            _logic.PositionChanged -= GetPositionChanged;
            GC.SuppressFinalize(this);
        }
    }
}
