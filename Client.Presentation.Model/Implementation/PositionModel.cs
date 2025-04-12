using TPUM.Client.Logic;

namespace TPUM.Client.Presentation.Model
{
    internal class PositionModel : IPositionModel
    {
        private readonly IPositionLogic _logic;

        public float X => _logic.X;

        public float Y => _logic.Y;

        public PositionModel(IPositionLogic logic)
        {
            _logic = logic;
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
            GC.SuppressFinalize(this);
        }
    }
}