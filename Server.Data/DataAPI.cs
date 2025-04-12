namespace TPUM.Server.Data
{
    public abstract class DataApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoomData> Rooms { get; }

        public abstract IRoomData AddRoom(string name, float width, float height);

        public abstract bool ContainsRoom(Guid roomId);

        public abstract IRoomData? GetRoom(Guid roomId);

        public abstract void RemoveRoom(Guid roomId);

        public abstract void Dispose();

        private static DataApiBase? _instance = null;

        public static DataApiBase GetApi()
        {
            return _instance ??= new DataApi();
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

        public override bool ContainsRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                return _rooms.Any(room => room.Id == roomId);
            }
        }

        public override IRoomData? GetRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                return _rooms.Find(room => room.Id == roomId);
            }
        }

        public override void RemoveRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == roomId);
                if (room != null) _rooms.Remove(room);
            }
        }

        private void ClearRooms()
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