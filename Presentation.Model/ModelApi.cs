using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TPUM.Logic;

namespace TPUM.Presentation.Model
{
    public abstract class ModelApiBase : IDisposable
    {
        public abstract ReadOnlyObservableCollection<IModelRoom> Rooms { get; }
        public abstract IModelRoom AddRoom(string name, float width, float height);
        public abstract void RemoveRoom(long id);
        public abstract ICommand CreateCommand(Action<object?> execute, Predicate<object?> canExecute);
        public abstract ICommand CreateCommand(Action<object?> execute);
        public abstract void Dispose();

        public static ModelApiBase GetApi(LogicApiBase? logic = null)
        {
            return new ModelApi(logic ?? LogicApiBase.GetApi());
        }
    }

    internal class ModelApi : ModelApiBase
    {
        private readonly LogicApiBase _logic;
        private readonly ObservableCollection<IModelRoom> _rooms = [];
        public override ReadOnlyObservableCollection<IModelRoom> Rooms { get; }

        public ModelApi(LogicApiBase logic)
        {
            _logic = logic;
            Rooms = new ReadOnlyObservableCollection<IModelRoom>(_rooms);
        }

        public override IModelRoom AddRoom(string name, float width, float height)
        {
            var room = new ModelRoom(name, _logic.AddRoom(width, height));
            _rooms.Add(room);
            return room;
        }

        public override void RemoveRoom(long id)
        {
            var room = _rooms.ToList().Find(room => room.Id == id);
            if (room != null) _rooms.Remove(room);
        }

        public override ICommand CreateCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            return new CustomCommand(execute, canExecute);
        }

        public override ICommand CreateCommand(Action<object?> execute)
        {
            return new CustomCommand(execute);
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
