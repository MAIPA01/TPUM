using TPUM.Logic;

namespace TPUM.Presentation.Model
{
    public abstract class ModelApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoom> Rooms { get; }
        public abstract IRoom AddRoom(string name, float width, float height);
        public abstract void RemoveRoom(long id);
        public abstract void Dispose();

        public static ModelApiBase GetApi(LogicApiBase? logic = null)
        {
            return new ModelApi(logic ?? LogicApiBase.GetApi());
        }
    }

    internal class ModelApi : ModelApiBase
    {
        private readonly LogicApiBase _logic;
        private readonly List<IRoom> _rooms = [];
        public override IReadOnlyCollection<IRoom> Rooms => _rooms.AsReadOnly();

        public ModelApi(LogicApiBase logic)
        {
            _logic = logic;
            foreach (var room in _logic.Rooms)
            {
                _rooms.Add(new Room("", room));
            }
        }

        public override IRoom AddRoom(string name, float width, float height)
        {
            var room = new Room(name, _logic.AddRoom(width, height));
            _rooms.Add(room);
            return room;
        }

        public override void RemoveRoom(long id)
        {
            var room = _rooms.First(room => room.Id == id);
            _rooms.Remove(room);
            _logic.RemoveRoom(id);
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