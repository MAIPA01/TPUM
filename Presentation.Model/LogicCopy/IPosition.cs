namespace TPUM.Presentation.Model
{
    public interface IPosition : INotifyPositionChanged, IDisposable
    {
        float X { get; set; }
        float Y { get; set; }

        static float Distance(IPosition pos1, IPosition pos2)
        {
            return MathF.Sqrt((pos1.X - pos2.X) * (pos1.X - pos2.X) + (pos1.Y - pos2.Y) * (pos1.Y - pos2.Y));
        }
    }
}