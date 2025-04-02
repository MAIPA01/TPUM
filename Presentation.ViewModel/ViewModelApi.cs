using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TPUM.Presentation.Model;

namespace TPUM.Presentation.ViewModel
{
    public abstract class ViewModelApiBase : IDisposable
    {
        public abstract ReadOnlyObservableCollection<IRoom> Rooms { get; }

        public abstract IRoom? CurrentRoom { get; }

        public abstract IRoom AddRoom(string name, float width, float height);

        public abstract void RemoveRoom(long id);

        public abstract ICommand CreateCommand(Action<object?> execute, Predicate<object?> canExecute);

        public abstract ICommand CreateCommand(Action<object?> execute);

        public abstract void SetCurrentRoom(long roomId);

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
            private set
            {
                _instance = value;
            }
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

        public override void RemoveRoom(long id)
        {
            var room = _rooms.First(room => room.Id == id);
            if (room != null) _rooms.Remove(room);
            _model.RemoveRoom(id);
        }

        public override ICommand CreateCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            return _model.CreateCommand(execute, canExecute);
        }

        public override ICommand CreateCommand(Action<object?> execute)
        {
            return _model.CreateCommand(execute);
        }

        public override void SetCurrentRoom(long roomId)
        {
            var room = _rooms.First(room => room.Id == roomId);
            if (room == null) return;
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
