using TPUM.Server.Logic;

namespace TPUM.Server.Presentation
{
    public abstract class PresentationApi
    {
        public abstract void AddRoom(string name, float width, float height);
        public abstract void UpdateRoom(string name, float width, float height);
        public abstract void SendRoom(Guid id);

        public abstract void AddHeater(Guid roomId, float x, float y, float temperature);
        public abstract void UpdateHeater();
        public abstract void SendHeater(Guid roomId, Guid id);

        public abstract void AddHeatSensor(Guid roomId, float x, float y);
        public abstract void SendHeatSensor(Guid roomId, Guid id);
    }
}
