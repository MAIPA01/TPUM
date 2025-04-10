using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    public interface IPositionModel : INotifyPositionChanged, IDisposable
    {
        float X { get; set; }
        float Y { get; set; }

        void SetPosition(float x, float y);
    }
}