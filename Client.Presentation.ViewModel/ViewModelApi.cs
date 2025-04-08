using System.Collections.ObjectModel;
using TPUM.Client.Presentation.Model;

namespace TPUM.Client.Presentation.ViewModel
{
    public abstract class ViewModelApiBase : IDisposable
    {
        public abstract ReadOnlyObservableCollection<IRoom> Rooms { get; }

        public abstract IRoom? CurrentRoom { get; }

        public abstract IRoom AddRoom(string name, float width, float height);

        public abstract void RemoveRoom(Guid id);

        public abstract void SetCurrentRoom(Guid roomId);

        public abstract void Dispose();

        public static ViewModelApiBase GetApi(ModelApiBase? model = null)
        {
            return new ViewModelApi(model ?? ModelApiBase.GetApi());
        }
    }

    internal class ViewModelApi : ViewModelApiBase
    {
        private static ViewModelApiBase? _instance = null;
        internal static ViewModelApiBase Instance
        {
            get
            {
                _instance ??= GetApi();
                return _instance;
            }
            private set => _instance = value;
        }

        private readonly ModelApiBase _model;

        private readonly ObservableCollection<IRoom> _rooms = [];
        public override ReadOnlyObservableCollection<IRoom> Rooms { get; }

        private IRoom? _currentRoom = null;
        public override IRoom? CurrentRoom => _currentRoom;

        public ViewModelApi(ModelApiBase model)
        {
            Instance = this;
            _model = model;

            Rooms = new ReadOnlyObservableCollection<IRoom>(_rooms);
            foreach (var room in _model.Rooms)
            {
                _rooms.Add(new Room(room));
            }
        }

        public override IRoom AddRoom(string name, float width, float height)
        {
            var room = new Room(_model.AddRoom(name, width, height));
            _rooms.Add(room);
            return room;
        }

        public override void RemoveRoom(Guid id)
        {
            var room = _rooms.First(room => room.Id == id);
            _rooms.Remove(room);
            _model.RemoveRoom(id);
        }

        public override void SetCurrentRoom(Guid roomId)
        {
            var room = _rooms.First(room => room.Id == roomId);
            _currentRoom = room;
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