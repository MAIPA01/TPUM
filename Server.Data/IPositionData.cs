using TPUM.Server.Data.Events;

namespace TPUM.Server.Data
{
    public interface IPositionData : INotifyPositionChanged, IDisposable
    {
        public float X { get; set; }
        public float Y { get; set; }

        void SetPosition(float x, float y);
    }
}