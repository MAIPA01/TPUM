namespace TPUM.Data
{
    public class Position(float x, float y) : IDisposable
    {
        public float X { get; set; } = x;
        public float Y { get; set; } = y;

        public static float Distance(Position pos1, Position pos2)
        {
            return MathF.Sqrt(pos1.X * pos2.X + pos1.Y * pos2.Y);
        }

        public static bool operator==(Position pos1, Position pos2)
        {
            return pos1.X == pos2.X && pos1.Y == pos2.Y;
        }

        public static bool operator!=(Position pos1, Position pos2)
        {
            return pos1.X != pos2.X || pos1.Y != pos2.Y;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Position) return false;
            return this == (Position)obj;
        }

        public override int GetHashCode()
        {
            return 3 * X.GetHashCode() + 5 * Y.GetHashCode();
        }

        public void Dispose() 
        {
            GC.SuppressFinalize(this);
        }
    }
}
