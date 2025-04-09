using TPUM.Server.Data;

namespace TPUM.Server.Logic
{
    public abstract class LogicApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoomLogic> Rooms { get; }

        public abstract IRoomLogic AddRoom(string name, float width, float height);

        public abstract bool ContainsRoom(Guid id);

        public abstract IRoomLogic? GetRoom(Guid id);

        public abstract void RemoveRoom(Guid id);

        public abstract void ClearRooms();

        public static LogicApiBase GetApi(DataApiBase? data = null)
        {
            return new LogicApi(data ?? DataApiBase.GetApi());
        }

        public abstract void Dispose();
    }

    internal class LogicApi : LogicApiBase
    {
        private readonly DataApiBase _data;
        private readonly object _roomsLock = new();
        private readonly List<RoomLogic> _rooms = [];

        public override IReadOnlyCollection<IRoomLogic> Rooms
        {
            get
            {
                lock (_roomsLock)
                {
                    return _rooms.AsReadOnly();
                }
            }
        }

        public LogicApi(DataApiBase data)
        {
            _data = data;
            foreach (var room in _data.Rooms)
            {
                _rooms.Add(new RoomLogic(room));
            }
        }

        public override IRoomLogic AddRoom(string name, float width, float height)
        {
            var room = new RoomLogic(_data.AddRoom(name, width, height));
            room.StartSimulation();
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

        public override IRoomLogic? GetRoom(Guid id)
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
                if (room != null)
                {
                    room.EndSimulation();
                    _rooms.Remove(room);
                }
            }
            _data.RemoveRoom(id);
        }

        public override void ClearRooms()
        {
            lock (_roomsLock)
            {
                foreach (var room in _rooms)
                {
                    room.EndSimulation();
                }
                _rooms.Clear();
            }
            _data.ClearRooms();
        }

        public override void Dispose()
        {
            ClearRooms();
            _data.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}