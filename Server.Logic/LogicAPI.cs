using Microsoft.VisualBasic;
using TPUM.Server.Data;

namespace TPUM.Server.Logic
{
    public abstract class LogicApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoomLogic> Rooms { get; }

        public abstract IRoomLogic AddRoom(string name, float width, float height);

        public abstract bool ContainsRoom(Guid roomId);

        public abstract IRoomLogic? GetRoom(Guid roomId);

        public abstract void RemoveRoom(Guid roomId);

        public abstract void Dispose();

        private static LogicApiBase? _instance = null;

        public static LogicApiBase GetApi(DataApiBase? data = null)
        {
            return _instance ??= new LogicApi(data ?? DataApiBase.GetApi());
        }
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

        public override bool ContainsRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                return _rooms.Any(room => room.Id == roomId);
            }
        }

        public override IRoomLogic? GetRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == roomId);
                if (room != null) return room;

                var dataRoom = _data.GetRoom(roomId);
                if (dataRoom == null) return null;

                room = new RoomLogic(dataRoom);
                room.StartSimulation();
                _rooms.Add(room);
                return room;
            }
        }

        public override void RemoveRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == roomId);
                if (room != null)
                {
                    room.EndSimulation();
                    _rooms.Remove(room);
                }
            }
            _data.RemoveRoom(roomId);
        }

        private void ClearRooms()
        {
            lock (_roomsLock)
            {
                foreach (var room in _rooms)
                {
                    room.EndSimulation();
                }
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