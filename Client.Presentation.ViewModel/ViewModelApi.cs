using System.Collections.ObjectModel;
using System.Windows;
using TPUM.Client.Presentation.Model;
using TPUM.Client.Presentation.ViewModel.Events;

namespace TPUM.Client.Presentation.ViewModel
{
    public abstract class ViewModelApiBase : INotifyClientConnected, INotifyRoomAdded, INotifyRoomRemoved, IDisposable
    {
        public abstract event ClientConnectedEventHandler? ClientConnected;
        public abstract event RoomAddedEventHandler? RoomAdded;
        public abstract event RoomRemovedEventHandler? RoomRemoved;

        public abstract ReadOnlyObservableCollection<IRoom> Rooms { get; }

        public abstract IRoom? CurrentRoom { get; }

        public abstract void AddRoom(string name, float width, float height);

        public abstract bool ContainsRoom(Guid roomId);

        public abstract IRoom? GetRoom(Guid roomId);

        public abstract void RemoveRoom(Guid id);

        public abstract void SetCurrentRoom(Guid roomId);

        public abstract void Refresh();

        public abstract void Dispose();

        private static ViewModelApiBase? _instance = null;
        public static ViewModelApiBase GetApi(ModelApiBase model)
        {
            return _instance ??= new ViewModelApi(model);
        }

        public static ViewModelApiBase GetApi(string? serverUri = null)
        {
            return _instance ??= GetApi(ModelApiBase.GetApi(serverUri ?? "ws://localhost:5000/ws"));
        }
    }

    internal class ViewModelApi : ViewModelApiBase
    {
        private readonly ModelApiBase _model;

        public override event ClientConnectedEventHandler? ClientConnected;
        public override event RoomAddedEventHandler? RoomAdded;
        public override event RoomRemovedEventHandler? RoomRemoved;

        private readonly object _roomsLock = new();
        private readonly ObservableCollection<IRoom> _rooms = [];
        public override ReadOnlyObservableCollection<IRoom> Rooms { get; }

        private IRoom? _currentRoom = null;
        public override IRoom? CurrentRoom => _currentRoom;

        public ViewModelApi(ModelApiBase model)
        {
            _model = model;

            Rooms = new ReadOnlyObservableCollection<IRoom>(_rooms);
            foreach (var room in _model.Rooms)
            {
                _rooms.Add(new Room(room));
            }

            _model.ClientConnected += GetClientConnected;
            _model.RoomAdded += GetRoomAdded;
            _model.RoomRemoved += GetRoomRemoved;
        }

        private void GetClientConnected(object? source)
        {
            ClientConnected?.Invoke(this);
        }

        private void GetRoomAdded(object? source, IRoomModel roomModel)
        {
            lock (_roomsLock)
            {
                var room = new Room(roomModel);
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _rooms.Add(room);
                });
                RoomAdded?.Invoke(this, room);
            }
        }

        private void GetRoomRemoved(object? source, Guid roomId)
        {
            lock (_roomsLock)
            {
                if (_rooms.Any(room => room.Id == roomId))
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _rooms.Remove(_rooms.First(room => room.Id == roomId));
                    });
                }
                RoomRemoved?.Invoke(this, roomId);
            }
        }

        public override void AddRoom(string name, float width, float height)
        {
            _model.AddRoom(name, width, height);
        }

        public override bool ContainsRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                return _rooms.Any(room => room.Id == roomId);
            }
        }

        public override IRoom? GetRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                var room = _rooms.FirstOrDefault(room => room.Id == roomId);
                if (room != null) return room;

                var modelRoom = _model.GetRoom(roomId);
                if (modelRoom == null) return null;

                room = new Room(modelRoom);
                Application.Current.Dispatcher.Invoke(() => _rooms.Add(room));
                return room;
            }
        }

        public override void RemoveRoom(Guid id)
        {
            _model.RemoveRoom(id);
        }

        public override void SetCurrentRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                _currentRoom = _rooms.FirstOrDefault(room => room.Id == roomId);
            }
        }

        public override void Refresh()
        {
            _model.Refresh();
        }

        public override void Dispose()
        {
            _model.ClientConnected -= GetClientConnected;
            _model.RoomAdded -= GetRoomAdded;
            _model.RoomRemoved -= GetRoomRemoved;

            _rooms.Clear();
            GC.SuppressFinalize(this);
        }
    }
}