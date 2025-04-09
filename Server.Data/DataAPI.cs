namespace TPUM.Server.Data
{
    public abstract class DataApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoomData> Rooms { get; }

        public abstract IPositionData CreatePosition(float x, float y);
        
        public abstract IHeaterData CreateHeater(float x, float y, float temperature);

        public abstract IHeatSensorData CreateHeatSensor(float x, float y);

        public abstract IRoomData AddRoom(float width, float height);

        public abstract void RemoveRoom(Guid id);

        public abstract void ClearRooms();

        public abstract void Dispose();

        public static DataApiBase GetApi()
        {
            return new DataApi();
        }
    }

    internal class DataApi : DataApiBase
    {
        private readonly object _roomsLock = new();
        private readonly List<IRoomData> _rooms = [];
        public override IReadOnlyCollection<IRoomData> Rooms
        {
            get
            {
                lock (_roomsLock)
                {
                    return _rooms.AsReadOnly();
                }
            }
        }

        public override IPositionData CreatePosition(float x, float y)
        {
            return new PositionData(x, y);
        }

        public override IHeaterData CreateHeater(float x, float y, float temperature)
        {
            return new HeaterData(Guid.NewGuid(), new PositionData(x, y), temperature);
        }

        public override IHeatSensorData CreateHeatSensor(float x, float y)
        {
            return new HeatSensorData(Guid.NewGuid(), new PositionData(x, y));
        }

        public override IRoomData AddRoom(float width, float height)
        {
            IRoomData room = new RoomData(Guid.NewGuid(), width, height);
            lock (_roomsLock)
            {
                _rooms.Add(room);
            }
            return room;
        }

        public override void RemoveRoom(Guid id)
        {
            lock (_roomsLock)
            {
                IRoomData? room = _rooms.Find(room => room.Id == id);
                if (room != null) _rooms.Remove(room);
            }
        }

        public override void ClearRooms()
        {
            lock (_roomsLock)
            {
                _rooms.Clear();
            }
        }

        public override void Dispose()
        {
            ClearRooms();
            GC.SuppressFinalize(this);
        }
    }
}