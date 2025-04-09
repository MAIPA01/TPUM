using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
{
    public interface IPositionLogic : INotifyPositionChanged, IDisposable
    {
        float X { get; set; }
        float Y { get; set; }

        static float Distance(IPositionLogic pos1, IPositionLogic pos2)
        {
            return MathF.Sqrt((pos1.X - pos2.X) * (pos1.X - pos2.X) + (pos1.Y - pos2.Y) * (pos1.Y - pos2.Y));
        }
    }
}