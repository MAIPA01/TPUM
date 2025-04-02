namespace TPUM.Data
{
    public interface IPosition : INotifyPositionChanged, IDisposable
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static float Distance(IPosition pos1, IPosition pos2)
        {
            return MathF.Sqrt((pos1.X - pos2.X) * (pos1.X - pos2.X) + (pos1.Y - pos2.Y) * (pos1.Y - pos2.Y));
        }
    }
}