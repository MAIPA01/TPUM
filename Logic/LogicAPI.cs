using System.Collections.ObjectModel;
using TPUM.Data;

namespace TPUM.Logic
{
    public abstract class LogicApiBase : IDisposable
    {
        public abstract ReadOnlyCollection<IRoom> Rooms { get; }

        public abstract IRoom AddRoom(float width, float height);

        public abstract void RemoveRoom(long id);

        public static LogicApiBase GetApi(DataApiBase? data = null)
        {
            return new LogicApi(data ?? DataApiBase.GetApi());
        }

        public abstract void Dispose();
    }

    internal class LogicApi(DataApiBase data) : LogicApiBase
    {
        private readonly List<IRoom> _rooms = [];
        public override ReadOnlyCollection<IRoom> Rooms => _rooms.AsReadOnly();

        public override IRoom AddRoom(float width, float height)
        {
            var room = new Room(new Random().NextInt64(), width, height, data);
            room.StartSimulation();
            _rooms.Add(room);
            return room;
        }

        public override void RemoveRoom(long id)
        {
            var room = _rooms.Find(room => room.Id == id);
            if (room == null) return;
            room.EndSimulation();
            _rooms.Remove(room);
        }

        public override void Dispose()
        {
            foreach (var room in _rooms)
            {
                room.EndSimulation();
            }
            data.Dispose();
            foreach (var room in _rooms)
            {
                room.Dispose();
            }
            _rooms.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
