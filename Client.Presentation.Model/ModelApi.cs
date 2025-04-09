using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    public abstract class ModelApiBase : IDisposable, INotifyClientConnected
    {
        public abstract IReadOnlyCollection<IRoom> Rooms { get; }

        public abstract event ClientConnectedEventHandler? ClientConnected;

        public abstract IRoom? AddRoom(string name, float width, float height);
        public abstract void RemoveRoom(Guid id);
        public abstract void Dispose();

        public static ModelApiBase GetApi(LogicApiBase logic)
        {
            return new ModelApi(logic);
        }

        public static ModelApiBase GetApi(string serverUri, string broadcastUri)
        {
            return new ModelApi(LogicApiBase.GetApi(serverUri, broadcastUri));
        }
    }

    internal class ModelApi : ModelApiBase
    {
        private readonly LogicApiBase _logic;
        private readonly List<IRoom> _rooms = [];

        public override event ClientConnectedEventHandler? ClientConnected;

        public override IReadOnlyCollection<IRoom> Rooms => _rooms.AsReadOnly();

        public ModelApi(LogicApiBase logic)
        {
            _logic = logic;
            foreach (var room in _logic.Rooms)
            {
                _rooms.Add(new Room(room, this));
            }

            _logic.HeaterChanged += OnHeaterChanged;
            _logic.HeatSensorChanged += OnHeatSensorChanged;
            _logic.RoomChanged += OnRoomChanged;
            _logic.ClientConnected += OnClientConnected;
        }

        internal IHeater? AddHeater(Guid roomId, float x, float y, float temperature)
        {
            var heaterLogic = _logic.AddHeater(roomId, x, y, temperature);
            if (heaterLogic == null) return null;

            var heater = new Heater(heaterLogic, this);
            return heater;
        }

        internal Guid FindRoomWithHeater(Guid heaterId)
        {
            var room = _rooms.Find(room => room.Heaters.Any(heater => heater.Id == heaterId));
            if (room == null) return Guid.Empty;
            return room.Id;
        }

        internal void UpdateHeater(Guid id, float x, float y, float temperature, bool isOn)
        {
            if (id == Guid.Empty) return;
            Guid roomId = FindRoomWithHeater(id);
            if (roomId == Guid.Empty) return;
            _logic.UpdateHeater(roomId, id, x, y, temperature, isOn);
        }

        internal void RemoveHeater(Guid roomId, Guid id)
        {
            _logic.RemoveHeater(roomId, id);
        }

        internal IHeatSensor? AddHeatSensor(Guid roomId, float x, float y)
        {
            var sensorLogic = _logic.AddHeatSensor(roomId, x, y);
            if (sensorLogic == null) return null;

            var sensor = new HeatSensor(sensorLogic);
            return sensor;
        }

        internal void RemoveHeatSensor(Guid roomId, Guid id)
        {
            _logic.RemoveHeatSensor(roomId, id);
        }

        public override IRoom? AddRoom(string name, float width, float height)
        {
            var roomLogic = _logic.AddRoom(name, width, height);
            if (roomLogic == null) return null;
            var room = new Room(roomLogic, this);
            _rooms.Add(room);
            return room;
        }

        private IRoom? GetRoom(Guid id)
        {
            return _rooms.Find(r => r.Id == id);
        }

        public override void RemoveRoom(Guid id)
        {
            var room = GetRoom(id);
            if (room != null)
            {
                room.Dispose();
                _rooms.Remove(room);
            }
            _logic.RemoveRoom(id);
        }

        public override void Dispose()
        {
            _logic.HeaterChanged -= OnHeaterChanged;
            _logic.HeatSensorChanged -= OnHeatSensorChanged;
            _logic.RoomChanged -= OnRoomChanged;
            _logic.ClientConnected -= OnClientConnected;

            foreach (var room in _rooms)
            {
                room.Dispose();
            }
            _rooms.Clear();

            _logic.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnHeaterChanged(object? source, Logic.Events.HeaterChangedEventArgs e)
        {
            IRoom? room = GetRoom(e.RoomId);
            if (room == null) return;

            if (e.Removed)
            {
                ((Room)room).RemoveHeaterFromList(e.Id);
                return;
            }

            if (e.Updated || (!e.Updated && !e.Removed))
            {
                var logic = _logic.GetHeater(e.RoomId, e.Id);
                if (logic == null) return;

                var heaterUpdate = ((Room)room).GetHeater(e.Id);
                if (heaterUpdate != null)
                {
                    ((Heater)heaterUpdate).UpdateData(logic.Position.X, logic.Position.Y, logic.Temperature, logic.IsOn);
                    return;
                }

                var h = new Heater(logic, this);
                ((Room)room).AddHeater(h);
                return;
            }
        }

        private void OnHeatSensorChanged(object? source, Logic.Events.HeatSensorChangedEventArgs e)
        {
            IRoom? room = GetRoom(e.RoomId);
            if (room == null) return;

            if (e.Removed)
            {
                ((Room)room).RemoveHeatSensorFromList(e.Id);
                return;
            }

            if (e.Updated || (!e.Updated && !e.Removed))
            {
                var logic = _logic.GetHeatSensor(e.RoomId, e.Id);
                if (logic == null) return;

                var sensorUpdate = ((Room)room).GetHeatSensor(e.Id);
                if (sensorUpdate != null)
                {
                    ((HeatSensor)sensorUpdate).UpdateData(logic.Position.X, logic.Position.Y, logic.Temperature);
                    return;
                }

                var s = new HeatSensor(logic);
                ((Room)room).AddHeatSensor(s);
                return;
            }
        }

        private void OnRoomChanged(object? source, Logic.Events.RoomChangedEventArgs e)
        {
            IRoom? room;
            if (e.Removed)
            {
                room = GetRoom(e.RoomId);
                if (room != null)
                {
                    room.Dispose();
                    _rooms.Remove(room);
                }
                return;
            }

            if (e.Updated || (!e.Updated && !e.Removed))
            {
                var logic = _logic.GetRoom(e.RoomId);
                if (logic == null) return;

                room = GetRoom(e.RoomId);
                if (room != null)
                {
                    ((Room)room).UpdateData(logic.Name, logic.Width, logic.Height, logic.AvgTemperature);
                    return;
                }

                _rooms.Add(new Room(logic, this));
                return;
            }
        }

        private void OnClientConnected(object? source, Logic.Events.ClientConnectedEventArgs e)
        {
            ClientConnected?.Invoke(source, ClientConnectedEventArgs.Empty);
        }
    }
}