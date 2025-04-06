namespace TPUM.Data
{
    public abstract class DataApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoom> Rooms { get; }

        public abstract IRoom AddRoom(float width, float height);

        public abstract void RemoveRoom(long id);

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
        private readonly List<IRoom> _rooms = [];
        public override IReadOnlyCollection<IRoom> Rooms
        {
            get
            {
                lock (_roomsLock)
                {
                    return _rooms.AsReadOnly();
                }
            }
        }

        public override IRoom AddRoom(float width, float height)
        {
            var room = new Room(new Random().NextInt64(), width, height);
            lock (_roomsLock)
            {
                _rooms.Add(room);
            }
            return room;
        }

        public override void RemoveRoom(long id)
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
            foreach (var room in _rooms)
            {
                room.Dispose();
            }
            _rooms.Clear();
            GC.SuppressFinalize(this);
        }
    }
}