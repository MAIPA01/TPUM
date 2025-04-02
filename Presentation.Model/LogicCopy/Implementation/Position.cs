namespace TPUM.Presentation.Model
{
    internal class Position : IPosition
    {
        public event PositionChangedEventHandler? PositionChanged;

        private readonly Logic.IPosition _position;

        public float X
        {
            get => _position.X;
            set => _position.X = value;
        }
        public float Y
        {
            get => _position.Y;
            set => _position.Y = value;
        }

        public Position(Logic.IPosition position)
        {
            _position = position;
            _position.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object? source, Logic.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), this));
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
            return _position.GetHashCode();
        }

        public void Dispose()
        {
            _position.PositionChanged -= GetPositionChanged;
            _position.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}