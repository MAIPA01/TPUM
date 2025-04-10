using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    public abstract class ModelApiBase : INotifyClientConnected, INotifyRoomAdded, INotifyRoomRemoved, IDisposable
    {
        public abstract IReadOnlyCollection<IRoomModel> Rooms { get; }

        public abstract event ClientConnectedEventHandler? ClientConnected;

        public abstract event RoomAddedEventHandler? RoomAdded;
        public abstract event RoomRemovedEventHandler? RoomRemoved;

        public abstract void AddRoom(string name, float width, float height);
        public abstract IRoomModel? GetRoom(Guid id);
        public abstract void RemoveRoom(Guid id);
        public abstract void Dispose();

        public static ModelApiBase GetApi(LogicApiBase logic)
        {
            return new ModelApi(logic);
        }

        public static ModelApiBase GetApi(string serverUri)
        {
            return GetApi(LogicApiBase.GetApi(serverUri));
        }
    }

    internal class ModelApi : ModelApiBase
    {
        private readonly LogicApiBase _logic;

        public override event ClientConnectedEventHandler? ClientConnected;

        public override event RoomAddedEventHandler? RoomAdded;
        public override event RoomRemovedEventHandler? RoomRemoved;

        private readonly object _roomsLock = new();
        private readonly List<IRoomModel> _rooms = [];
        public override IReadOnlyCollection<IRoomModel> Rooms
        {
            get
            {
                lock (_roomsLock)
                {
                    return _rooms.AsReadOnly();
                }
            }
        }

        public ModelApi(LogicApiBase logic)
        {
            _logic = logic;
            foreach (var room in _logic.Rooms)
            {
                _rooms.Add(new RoomModel(room));
            }

            _logic.ClientConnected += GetClientConnected;
            _logic.RoomAdded += GetRoomAdded;
            _logic.RoomRemoved += GetRoomRemoved;
        }

        private void GetClientConnected(object? source)
        {
            ClientConnected?.Invoke(this);
        }

        private void GetRoomAdded(object? source, IRoomLogic roomLogic)
        {
            lock (_roomsLock)
            {
                var room = new RoomModel(roomLogic);
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
            _logic.AddRoom(name, width, height);
        }

        public override IRoomModel? GetRoom(Guid id)
        {
            lock (_roomsLock)
            {
                return _rooms.Find(r => r.Id == id);
            }
        }

        public override void RemoveRoom(Guid id)
        {
            _logic.RemoveRoom(id);
        }

        public override void Dispose()
        {
            _logic.ClientConnected -= GetClientConnected;
            _logic.RoomAdded -= GetRoomAdded;
            _logic.RoomRemoved -= GetRoomRemoved;
            _rooms.Clear();
            GC.SuppressFinalize(this);
        }
    }
}