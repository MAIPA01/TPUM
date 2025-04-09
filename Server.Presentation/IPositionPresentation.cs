using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    public interface IPositionPresentation : INotifyPositionChanged, IDisposable
    {
        float X { get; set; }
        float Y { get; set; }

        void SetPosition(float x, float y);
    }
}
