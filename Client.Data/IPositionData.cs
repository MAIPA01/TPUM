using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    public interface IPositionData : INotifyPositionChanged
    {
        public float X { get; set; }
        public float Y { get; set; }

        void SetPosition(float x, float y);
    }
}