using TPUM.Data;

namespace TPUM.Logic
{
    public abstract class LogicAPIBase : IDisposable
    {
        public abstract void AddRoom(float width, float height);

        public static LogicAPIBase GetAPI(DataAPIBase? data)
        {
            return new LogicAPI(data ?? DataAPIBase.GetAPI());
        }

        public abstract void Dispose();
    }

    internal class LogicAPI(DataAPIBase? data) : LogicAPIBase
    {
        private readonly DataAPIBase _data = data ?? DataAPIBase.GetAPI();

        public List<IRoom> Rooms { get; private set; } = [];

        public override void AddRoom(float width, float height)
        {
            Rooms.Add(new Room(width, height, _data));
        }

        public void UpdateAllRoomsTemperature(float deltaTime)
        {
            foreach (var room in Rooms)
            {
                room.UpdateTemperature(deltaTime);
            }
        }

        public override void Dispose()
        {
            _data.Dispose();
            foreach (var room in Rooms)
            {
                room.Dispose();
            }
            Rooms.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
