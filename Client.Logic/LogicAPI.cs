using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    public abstract class LogicApiBase : IDisposable, INotifyHeaterChanged, INotifyHeatSensorChanged, INotifyRoomChanged, INotifyClientConnected
    {
        public abstract IReadOnlyCollection<IRoomLogic> Rooms { get; }

        public abstract event HeaterChangedEventHandler? HeaterChanged;
        public abstract event HeatSensorChangedEventHandler? HeatSensorChanged;
        public abstract event RoomChangedEventHandler? RoomChanged;
        public abstract event ClientConnectedEventHandler? ClientConnected;

        public abstract IHeaterLogic? AddHeater(Guid roomId, float x, float y, float temperature);

        public abstract IHeaterLogic? GetHeater(Guid roomId, Guid id);

        public abstract void UpdateHeater(Guid roomId, Guid id, float x, float y, float temperature, bool isOn);

        public abstract void RemoveHeater(Guid roomId, Guid id);

        public abstract IHeatSensorLogic? AddHeatSensor(Guid roomId, float x, float y);

        public abstract IHeatSensorLogic? GetHeatSensor(Guid roomId, Guid id);

        public abstract void UpdateHeatSensor(Guid roomId, Guid id, float x, float y);

        public abstract void RemoveHeatSensor(Guid roomId, Guid id);

        public abstract IRoomLogic? AddRoom(string name, float width, float height);

        public abstract IRoomLogic? GetRoom(Guid id);

        public abstract void UpdateRoom(Guid id, string name, float width, float height);

        public abstract void RemoveRoom(Guid id);

        public abstract void Dispose();

        public static LogicApiBase GetApi(DataApiBase data)
        {
            return new LogicApi(data);
        }

        public static LogicApiBase GetApi(string serverUri)
        {
            return new LogicApi(DataApiBase.GetApi(serverUri));
        }
    }

    internal class LogicApi : LogicApiBase
    {
        private readonly DataApiBase _data;
        private readonly List<IRoomLogic> _rooms = [];
        public override IReadOnlyCollection<IRoomLogic> Rooms => _rooms;

        public override event RoomChangedEventHandler? RoomChanged;
        public override event HeaterChangedEventHandler? HeaterChanged;
        public override event HeatSensorChangedEventHandler? HeatSensorChanged;
        public override event ClientConnectedEventHandler? ClientConnected;

        public LogicApi(DataApiBase data)
        {
            _data = data;
            foreach (var room in _data.Rooms)
            {
                _rooms.Add(new RoomLogic(room));
            }

            _data.RoomChanged += OnRoomChanged;
            _data.HeaterChanged += OnHeaterChanged;
            _data.HeatSensorChanged += OnHeatSensorChanged;
            _data.ClientConnected += OnClientConnected;
        }

        private TLogic? TryAddToRoom<TLogic, TData>(Guid roomId, Func<Guid, TData?> createData, Func<TData, TLogic> createLogic, Func<IRoomLogic, TLogic, TLogic> addToRoom)
            where TLogic : class
            where TData : class
        {
            var data = createData(roomId);
            if (data == null) return null;

            var room = GetRoom(roomId);
            if (room == null) return null;

            var logic = createLogic(data);
            return addToRoom(room, logic);
        }

        public override IHeaterLogic? AddHeater(Guid roomId, float x, float y, float temperature)
        {
            return TryAddToRoom<IHeaterLogic, IHeaterData>
            (
                roomId,
                (id) => _data.AddHeater(id, x, y, temperature),
                (data) => new HeaterLogic(data),
                (room, logic) => room.AddHeater(logic)
            );
        }

        public override IHeaterLogic? GetHeater(Guid roomId, Guid id)
        {
            var room = GetRoom(roomId);
            if (room == null) return null;

            var heater = room.GetHeater(id);
            if (heater == null)
            {
                var heaterData = _data.GetHeater(roomId, id);
                if (heaterData == null) return null;
                heater = new HeaterLogic(heaterData);
                heater = room.AddHeater(heater);
            }

            return heater;
        }

        public override void UpdateHeater(Guid roomId, Guid id, float x, float y, float temperature, bool isOn)
        {
            var res = GetHeater(roomId, id);
            if (res == null) return;

            _data.UpdateHeater(roomId, id, x, y, temperature, isOn);
        }

        public override void RemoveHeater(Guid roomId, Guid id)
        {
            var res = GetRoom(roomId);
            if (res == null) return;

            res.RemoveHeater(id);
            _data.RemoveHeater(roomId, id);
        }

        public override IHeatSensorLogic? AddHeatSensor(Guid roomId, float x, float y)
        {
            return TryAddToRoom<IHeatSensorLogic, IHeatSensorData>
            (
                roomId,
                (id) => _data.AddHeatSensor(id, x, y),
                (data) => new HeatSensorLogic(data),
                (room, logic) => room.AddHeatSensor(logic)
            );
        }

        public override IHeatSensorLogic? GetHeatSensor(Guid roomId, Guid id)
        {
            var room = GetRoom(roomId);
            if (room == null) return null;

            var sensor = room.GetHeatSensor(id);
            if (sensor == null)
            {
                var sensorData = _data.GetHeatSensor(roomId, id);
                if (sensorData == null) return null;
                sensor = new HeatSensorLogic(sensorData);
                sensor = room.AddHeatSensor(sensor);
            }

            return sensor;
        }

        public override void UpdateHeatSensor(Guid roomId, Guid id, float x, float y)
        {
            var res = GetHeatSensor(roomId, id);
            if (res == null) return;

            _data.UpdateHeatSensor(roomId, id, x, y);
        }

        public override void RemoveHeatSensor(Guid roomId, Guid id)
        {
            var res = GetRoom(roomId);
            if (res == null) return;

            res.RemoveHeatSensor(id);
            _data.RemoveHeatSensor(roomId, id);
        }

        public override IRoomLogic? AddRoom(string name, float width, float height)
        {
            IRoomData? data = _data.AddRoom(name, width, height);
            if (data == null) return null;

            IRoomLogic room = new RoomLogic(data);
            _rooms.Add(room);
            return room;
        }

        public override IRoomLogic? GetRoom(Guid id)
        {
            var res = _rooms.Find(room => room.Id == id);
            if (res == null)
            {
                var data = _data.GetRoom(id);
                if (data == null) return null;

                res = new RoomLogic(data);
                _rooms.Add(res);
            }
            return res;
        }

        public override void UpdateRoom(Guid id, string name, float width, float height)
        {
            var res = GetRoom(id);
            if (res == null) return;

            _data.UpdateRoom(id, name, width, height);
        }

        public override void RemoveRoom(Guid id)
        {
            var res = GetRoom(id);
            if (res == null) return;

            _data.RemoveRoom(id);
            _rooms.Remove(res);
        }

        public override void Dispose()
        {
            _data.RoomChanged -= OnRoomChanged;
            _data.HeaterChanged -= OnHeaterChanged;
            _data.HeatSensorChanged -= OnHeatSensorChanged;
            _data.ClientConnected += OnClientConnected;

            _rooms.ForEach(r => r.Dispose());
            _rooms.Clear();

            _data.Dispose();
            GC.SuppressFinalize(this);
        }

        private void HandleRoomChanged(Guid roomId, bool updated, bool removed)
        {
            if (removed)
            {
                var room = _rooms.Find(r => r.Id == roomId);
                if (room != null)
                {
                    room.Dispose();
                    _rooms.Remove(room);
                }
                return;
            }

            if (updated || (!updated && !removed))
            {
                var dataRoom = _data.GetRoom(roomId);
                if (dataRoom == null) return;

                var room = _rooms.Find(r => r.Id == roomId);
                if (room == null)
                {
                    room = new RoomLogic(dataRoom);
                    _rooms.Add(room);
                }
            }
        }

        private void OnRoomChanged(object? source, Data.Events.RoomChangedEventArgs e)
        {
            HandleRoomChanged(e.RoomId, e.Updated, e.Removed);
            RoomChanged?.Invoke(source, new RoomChangedEventArgs(e.RoomId, e.Updated, e.Removed));
        }

        private void HandleHeaterChanged(Guid id, Guid roomId, bool updated, bool removed)
        {
            if (removed)
            {
                var room = _rooms.Find(r => r.Id == roomId);
                if (room != null)
                {
                    room.RemoveHeater(id);
                }
                return;
            }

            if (updated || (!updated && !removed))
            {
                var room = GetRoom(roomId);
                if (room == null) return;

                var heater = room.GetHeater(id);
                if (heater != null) return;

                var dataHeater = _data.GetHeater(roomId, id);
                if (dataHeater == null) return;

                heater = new HeaterLogic(dataHeater);
                room.AddHeater(heater);
            }
        }

        private void OnHeaterChanged(object? source, Data.Events.HeaterChangedEventArgs e)
        {
            HandleHeaterChanged(e.Id, e.RoomId, e.Updated, e.Removed);
            HeaterChanged?.Invoke(source, new HeaterChangedEventArgs(e.Id, e.RoomId, e.Updated, e.Removed));
        }

        private void HandleHeatSensorChanged(Guid id, Guid roomId, bool updated, bool removed)
        {
            if (removed)
            {
                var room = _rooms.Find(r => r.Id == roomId);
                if (room != null)
                {
                    room.RemoveHeatSensor(id);
                }
                return;
            }

            if (updated || (!updated && !removed))
            {
                var room = GetRoom(roomId);
                if (room == null) return;

                var sensor = room.GetHeatSensor(id);
                if (sensor != null) return;

                var dataSensor = _data.GetHeatSensor(roomId, id);
                if (dataSensor == null) return;

                sensor = new HeatSensorLogic(dataSensor);
                room.AddHeatSensor(sensor);
            }
        }

        private void OnHeatSensorChanged(object? source, Data.Events.HeatSensorChangedEventArgs e)
        {
            HandleHeatSensorChanged(e.Id, e.RoomId, e.Updated, e.Removed);
            HeatSensorChanged?.Invoke(source, new HeatSensorChangedEventArgs(e.Id, e.RoomId, e.Updated, e.Removed));
        }

        private void OnClientConnected(object? source, Data.Events.ClientConnectedEventArgs e)
        {
            ClientConnected?.Invoke(source, ClientConnectedEventArgs.Empty);
        }
    }
}