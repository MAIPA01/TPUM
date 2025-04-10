using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    public abstract class LogicApiBase : INotifyClientConnected, INotifyRoomAdded, INotifyRoomRemoved, IDisposable  
    {
        public abstract IReadOnlyCollection<IRoomLogic> Rooms { get; }

        public abstract event ClientConnectedEventHandler? ClientConnected;
        public abstract event RoomAddedEventHandler? RoomAdded;
        public abstract event RoomRemovedEventHandler? RoomRemoved;

        public abstract void AddRoom(string name, float width, float height);

        public abstract IRoomLogic? GetRoom(Guid id);

        public abstract void RemoveRoom(Guid id);

        public abstract void Dispose();

        public static LogicApiBase GetApi(DataApiBase data)
        {
            return new LogicApi(data);
        }

        public static LogicApiBase GetApi(string serverUri)
        {
            // TODO: zrobić singletone?
            return new LogicApi(DataApiBase.GetApi(serverUri));
        }
    }

    internal class LogicApi : LogicApiBase
    {
        private readonly DataApiBase _data;

        private readonly object _roomsLock = new();
        private readonly List<IRoomLogic> _rooms = [];
        public override IReadOnlyCollection<IRoomLogic> Rooms => _rooms;

        public override event ClientConnectedEventHandler? ClientConnected;
        public override event RoomAddedEventHandler? RoomAdded;
        public override event RoomRemovedEventHandler? RoomRemoved;

        public LogicApi(DataApiBase data)
        {
            _data = data;
            foreach (var room in _data.Rooms)
            {
                _rooms.Add(new RoomLogic(room));
            }

            _data.ClientConnected += GetClientConnected;
            _data.RoomAdded += GetRoomAdded;
            _data.RoomRemoved += GetRoomRemoved;
        }

        private void GetClientConnected(object? source)
        {
            ClientConnected?.Invoke(this);
        }

        private void GetRoomAdded(object? source, IRoomData roomData)
        {
            lock (_roomsLock)
            {
                var room = new RoomLogic(roomData);
                _rooms.Add(room);
                RoomAdded?.Invoke(this, room);
            }
        }

        private void GetRoomRemoved(object? source, Guid roomId)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == roomId);
                if (room != null) _rooms.Remove(room);
                RoomRemoved?.Invoke(this, roomId);
            }
        }

        public override void AddRoom(string name, float width, float height)
        {
            _data.AddRoom(name, width, height);
        }

        public override IRoomLogic? GetRoom(Guid id)
        {
            return _rooms.Find(room => room.Id == id);
        }

        public override void RemoveRoom(Guid id)
        {
            _data.RemoveRoom(id);
        }

        public override void Dispose()
        {
            _data.ClientConnected += GetClientConnected;
            _data.RoomAdded -= GetRoomAdded;
            _data.RoomRemoved -= GetRoomRemoved;

            _rooms.Clear();

            _data.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}