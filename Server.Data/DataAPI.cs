namespace TPUM.Server.Data
{
    public abstract class DataApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoomData> Rooms { get; }

        public abstract IRoomData AddRoom(string name, float width, float height);

        public abstract bool ContainsRoom(Guid id);

        public abstract IRoomData? GetRoom(Guid id);

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
        private readonly List<RoomData> _rooms = [];
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

        public override IRoomData AddRoom(string name, float width, float height)
        {
            var room = new RoomData(Guid.NewGuid(), name, width, height);
            lock (_roomsLock)
            {
                _rooms.Add(room);
            }
            return room;
        }

        public override bool ContainsRoom(Guid id)
        {
            lock (_roomsLock)
            {
                return _rooms.Find(room => room.Id == id) != null;
            }
        }

        public override IRoomData? GetRoom(Guid id)
        {
            lock (_roomsLock)
            {
                return _rooms.Find(room => room.Id == id);
            }
        }

        public override void RemoveRoom(Guid id)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == id);
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