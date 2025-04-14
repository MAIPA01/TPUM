using TPUM.Server.Logic;

namespace TPUM.Server.Presentation
{
    internal class Position : IPosition
    {
        private readonly IPositionLogic _logic;

        public float X => _logic.X;

        public float Y => _logic.Y;

        public Position(IPositionLogic logic)
        {
            _logic = logic;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}