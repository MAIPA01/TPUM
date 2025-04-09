using TPUM.Server.Data;

namespace TPUM.Server.Logic
{
    public abstract class LogicApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoomLogic> Rooms { get; }

        public abstract IPositionLogic CreatePosition(float x, float y);

        public abstract IHeaterLogic CreateHeater(float x, float y, float temperature);

        public abstract IHeatSensorLogic CreateHeatSensor(float x, float y);

        public abstract IRoomLogic AddRoom(float width, float height);

        public abstract bool ContainsRoom(Guid id);

        public abstract IRoomLogic? GetRoom(Guid id);

        public abstract void RemoveRoom(Guid id);

        public abstract void ClearRooms();

        public static LogicApiBase GetApi(DataApiBase? data = null)
        {
            return new LogicApi(data ?? DataApiBase.GetApi());
        }

        public abstract void Dispose();
    }

    internal class LogicApi : LogicApiBase
    {
        private readonly DataApiBase _data;
        private readonly List<IRoomLogic> _rooms = [];
        public override IReadOnlyCollection<IRoomLogic> Rooms => _rooms;

        public LogicApi(DataApiBase data)
        {
            _data = data;
            foreach (var room in _data.Rooms)
            {
                _rooms.Add(new RoomLogic(room));
            }
        }

        public override IPositionLogic CreatePosition(float x, float y)
        {
            return new PositionLogic(_data.CreatePosition(x, y));
        }

        public override IHeaterLogic CreateHeater(float x, float y, float temperature)
        {
            return new HeaterLogic(_data.CreateHeater(x, y, temperature));
        }

        public override IHeatSensorLogic CreateHeatSensor(float x, float y)
        {
            return new HeatSensorLogic(_data.CreateHeatSensor(x, y));
        }

        public override IRoomLogic AddRoom(float width, float height)
        {
            var room = new RoomLogic(_data.AddRoom(width, height));
            room.StartSimulation();
            _rooms.Add(room);
            return room;
        }

        public override bool ContainsRoom(Guid id)
        {
            return _data.ContainsRoom(id);
        }

        public override IRoomLogic? GetRoom(Guid id)
        {
            return _rooms.Find(room => room.Id == id);
        }

        public override void RemoveRoom(Guid id)
        {
            var room = _rooms.Find(room => room.Id == id);
            if (room != null)
            {
                room.EndSimulation();
                _rooms.Remove(room);
            }
            _data.RemoveRoom(id);
        }

        public override void ClearRooms()
        {
            _rooms.Clear();
            _data.ClearRooms();
        }

        public override void Dispose()
        {
            foreach (var room in _rooms)
            {
                room.EndSimulation();
            }
            _data.Dispose();
            foreach (var room in _rooms)
            {
                room.Dispose();
            }
            _rooms.Clear();
            GC.SuppressFinalize(this);
        }
    }
}