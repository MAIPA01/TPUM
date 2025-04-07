namespace TPUM.Client.Logic.Tests
{
    internal class DummyPosition : IPosition
    {
        private readonly Data.IPosition _position;

        public event PositionChangedEventHandler? PositionChanged;

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

        public DummyPosition(Data.IPosition position)
        {
            _position = position;
            _position.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object? source, Data.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), this));
        }

        public static bool operator ==(DummyPosition pos1, IPosition pos2)
        {
            return Math.Abs(pos1.X - pos2.X) < 1e-10f && Math.Abs(pos1.Y - pos2.Y) < 1e-10f;
        }

        public static bool operator !=(DummyPosition pos1, IPosition pos2)
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
            GC.SuppressFinalize(this);
        }
    }
}