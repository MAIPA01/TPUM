using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    public interface IPositionLogic : INotifyPositionChanged, IDisposable
    {
        float X { get; set; }
        float Y { get; set; }

        void SetPosition(float x, float y);
    }
}