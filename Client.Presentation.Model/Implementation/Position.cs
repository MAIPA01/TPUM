using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    internal class Position : IPosition
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

        public Position(IPositionLogic logic)
        {
            _logic = logic;
            _logic.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object? source, Logic.Events.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), this));
        }

        private void GetPositionChanged(object? source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(args.LastPosition, this));
        }

        public static bool operator ==(Position pos1, IPosition pos2)
        {
            return Math.Abs(pos1.X - pos2.X) < 1e-10f && Math.Abs(pos1.Y - pos2.Y) < 1e-10f;
        }

        public static bool operator !=(Position pos1, IPosition pos2)
        {
            return Math.Abs(pos1.X - pos2.X) > 1e-10f || Math.Abs(pos1.Y - pos2.Y) > 1e-10f;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not IPosition position) return false;
            return this == position;
        }

        public override int GetHashCode()
        {
            return _logic.GetHashCode();
        }

        public void Dispose()
        {
            _logic.PositionChanged -= GetPositionChanged;
            _logic.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}