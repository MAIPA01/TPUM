using TPUM.Client.Logic;

namespace TPUM.Client.Presentation.Model
{
    public abstract class ModelApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoom> Rooms { get; }
        public abstract IPosition CreatePosition(float x, float y);
        public abstract IHeater CreateHeater(float x, float y, float temperature);
        public abstract IHeatSensor CreateHeatSensor(float x, float y);
        public abstract IRoom AddRoom(string name, float width, float height);
        public abstract void RemoveRoom(Guid id);
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

        public override IPosition CreatePosition(float x, float y)
        {
            return new Position(_logic.CreatePosition(x, y));
        }

        public override IHeater CreateHeater(float x, float y, float temperature)
        {
            return new Heater(_logic.CreateHeater(x, y, temperature));
        }

        public override IHeatSensor CreateHeatSensor(float x, float y)
        {
            return new HeatSensor(_logic.CreateHeatSensor(x, y));
        }

        public override IRoom AddRoom(string name, float width, float height)
        {
            var room = new Room(name, _logic.AddRoom(width, height));
            _rooms.Add(room);
            return room;
        }

        public override void RemoveRoom(Guid id)
        {
            var room = _rooms.Find(room => room.Id == id);
            if (room != null)
            {
                _rooms.Remove(room);
            }
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