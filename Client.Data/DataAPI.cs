using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    public abstract class DataApiBase : IDisposable, INotifyRoomRemoved, INotifyRoomAdded, INotifyClientConnected
    {
        public abstract IReadOnlyCollection<IRoomData> Rooms { get; }

        public abstract event ClientConnectedEventHandler? ClientConnected;
        public abstract event RoomRemovedEventHandler? RoomRemoved;
        public abstract event RoomAddedEventHandler? RoomAdded;

        public abstract void AddRoom(string name, float width, float height);

        public abstract IRoomData? GetRoom(Guid id);

        public abstract void RemoveRoom(Guid id);

        public abstract void Dispose();

        public static DataApiBase GetApi(string serverUri)
        {
            // TODO: zrobiæ singleton
            return new DataApi(serverUri);
        }
    }

    internal delegate void DataApiHeaterAddedEventHandler(object? source, Guid roomId, IHeaterData heater);
    internal delegate void DataApiHeaterRemovedEventHandler(object? source, Guid roomId, Guid heaterId);
    internal delegate void DataApiHeatSensorAddedEventHandler(object? source, Guid roomId, IHeatSensorData sensor);
    internal delegate void DataApiHeatSensorRemovedEventHandler(object? source, Guid roomId, Guid sensorId);

    internal class DataApi : DataApiBase
    {
        private readonly WebSocketClient _client;
        private bool _connected = false;

        public override event ClientConnectedEventHandler? ClientConnected;
        public override event RoomRemovedEventHandler? RoomRemoved;
        public override event RoomAddedEventHandler? RoomAdded;

        public event DataApiHeaterAddedEventHandler? HeaterAdded;
        public event DataApiHeaterRemovedEventHandler? HeaterRemoved;
        public event DataApiHeatSensorAddedEventHandler? HeatSensorAdded;
        public event DataApiHeatSensorRemovedEventHandler? HeatSensorRemoved;

        private readonly object _roomsLock = new();
        private readonly List<IRoomData> _rooms = [];
        public override IReadOnlyCollection<IRoomData> Rooms
        {
            get
            {
                lock (_roomsLock)
                {
                    return _rooms.AsReadOnly();
                }
            }
        }

        public DataApi(string serverUri)
        {
            _client = new WebSocketClient();
            _client.ClientConnected += OnClientConnected;
            _client.ResponseReceived += HandleResponse;
            _client.BroadcastReceived += HandleBroadcast;
            _ = _client.ConnectAsync(serverUri);
        }

        private void SendRequest<TRequest>(TRequest request)
        {
            var xml = XmlSerializerHelper.Serialize(request);
            _ = _client.SendAsync(xml);
        }

        internal void AddHeater(Guid roomId, float x, float y, float temperature)
        {
            if (!_connected) return;

            var room = GetRoom(roomId);
            if (room == null) return;

            var req = new AddHeaterRequest { RoomId = roomId, X = x, Y = y, Temperature = temperature };
            SendRequest(req);
        }

        internal void UpdateHeater(Guid roomId, Guid id, float x, float y, float temperature, bool isOn)
        {
            if (!_connected) return;

            var room = GetRoom(roomId);
            if (room == null) return;

            var req = new UpdateHeaterRequest { Id = id, RoomId = roomId, X = x, Y = y, Temperature = temperature, IsOn = isOn };
            SendRequest(req);
        }

        internal void RemoveHeater(Guid roomId, Guid id)
        {
            if (!_connected) return;

            var room = GetRoom(roomId);
            if (room == null) return;

            var req = new RemoveHeaterRequest { Id = id, RoomId = roomId };
            SendRequest(req);
        }

        internal void AddHeatSensor(Guid roomId, float x, float y)
        {
            if (!_connected) return;

            var room = GetRoom(roomId);
            if (room == null) return;

            var req = new AddHeatSensorRequest { RoomId = roomId, X = x, Y = y };
            SendRequest(req);
        }

        internal void UpdateHeatSensor(Guid roomId, Guid id, float x, float y)
        {
            if (!_connected) return;

            var room = GetRoom(roomId);
            if (room == null) return;

            var req = new UpdateHeatSensorRequest { Id = id, RoomId = roomId, X = x, Y = y };
            SendRequest(req);
        }

        internal void RemoveHeatSensor(Guid roomId, Guid id)
        {
            if (!_connected) return;

            var room = GetRoom(roomId);
            if (room == null) return;

            var req = new RemoveHeatSensorRequest { Id = id, RoomId = roomId };
            SendRequest(req);
        }

        public override void AddRoom(string name, float width, float height)
        {
            if (!_connected) return;

            var req = new AddRoomRequest { Name = name, Width = width, Height = height };
            SendRequest(req);
        }

        public override IRoomData? GetRoom(Guid id)
        {
            lock (_roomsLock)
            {
                return _rooms.Find(r => r.Id == id);
            }
        }

        public override void RemoveRoom(Guid id)
        {
            if (!_connected) return;

            var req = new RemoveRoomRequest { Id = id };
            SendRequest(req);
        }

        private void ClearAllData()
        {
            lock (_roomsLock)
            {
                foreach (var room in _rooms)
                {
                    RoomRemoved?.Invoke(this, room.Id);
                }
                _rooms.Clear();
            }
        }

        private void GetAllData()
        {
            var req = new AllDataRequest { WantAll = true };
            SendRequest(req);
        }

        private void DoAddHeater(Guid roomId, Guid id, float x, float y, float temperature, bool isOn)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(r => r.Id == roomId);
                if (room == null) return;

                if (room.GetHeater(roomId) != null) return;

                var heater = new HeaterData(
                    this,
                    roomId,
                    id,
                    new PositionData(x, y),
                    temperature,
                    isOn
                );
                OnHeaterAdded(roomId, heater);
            }
        }

        private void DoUpdateHeater(Guid roomId, Guid id, float x, float y, float temperature, bool isOn)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(r => r.Id == roomId);
                if (room == null) return;

                var heater = room.GetHeater(id);
                if (heater == null)
                {
                    DoAddHeater(room.Id, id, x, y, temperature, isOn);
                    return;
                }

                heater.Position.SetPosition(x, y);
                heater.Temperature = temperature;
                heater.IsOn = isOn;
            }
        }

        private void DoRemoveHeater(Guid roomId, Guid id)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(r => r.Id == roomId);
                if (room == null) return;

                OnHeaterRemoved(roomId, id);
            }
        }

        private void DoAddHeatSensor(Guid roomId, Guid id, float x, float y, float temperature)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(r => r.Id == roomId);
                if (room == null) return;

                if (room.GetHeatSensor(roomId) != null) return;

                var sensor = new HeatSensorData(
                    this,
                    roomId,
                    id,
                    new PositionData(x, y),
                    temperature
                );
                OnHeatSensorAdded(roomId, sensor);
            }
        }

        private void DoUpdateHeatSensor(Guid roomId, Guid id, float x, float y, float temperature)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(r => r.Id == roomId);
                if (room == null) return;

                var sensor = room.GetHeatSensor(id);
                if (sensor == null)
                {
                    DoAddHeatSensor(room.Id, id, x, y, temperature);
                    return;
                }

                sensor.Position.SetPosition(x, y);
                (sensor as HeatSensorData)?.SetTemperature(temperature);
            }
        }

        private void DoRemoveHeatSensor(Guid roomId, Guid id)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(r => r.Id == roomId);
                if (room == null) return;

                OnHeatSensorRemoved(roomId, id);
            }
        }

        private void DoAddRoom(RoomBroadcast roomRes)
        {
            lock (_roomsLock)
            {
                if (_rooms.Find(room => room.Id == roomRes.Id) != null) return;

                var room = new RoomData(this, roomRes.Id, roomRes.Name, roomRes.Width, roomRes.Height);

                foreach (var heaterDto in roomRes.Heaters)
                {
                    DoAddHeater(roomRes.Id, heaterDto.Id, heaterDto.X, heaterDto.Y, heaterDto.Temperature, heaterDto.IsOn);
                }

                foreach (var sensorDto in roomRes.HeatSensors)
                {
                    DoAddHeatSensor(roomRes.Id, sensorDto.Id, sensorDto.X, sensorDto.Y, sensorDto.Temperature);
                }

                _rooms.Add(room);
                OnRoomAdded(room);
            }
        }

        private void DoUpdateRoom(RoomBroadcast roomRes)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == roomRes.Id);
                if (room == null)
                {
                    DoAddRoom(roomRes);
                    return;
                }


                foreach (var heaterDto in roomRes.Heaters)
                {
                    DoUpdateHeater(room.Id, heaterDto.Id, heaterDto.X, heaterDto.Y, heaterDto.Temperature, heaterDto.IsOn);
                }

                foreach (var sensorDto in roomRes.HeatSensors)
                {
                    DoUpdateHeatSensor(room.Id, sensorDto.Id, sensorDto.X, sensorDto.Y, sensorDto.Temperature);
                }
            }
        }

        private void DoRemoveRoom(RoomBroadcast roomRes)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == roomRes.Id);
                if (room != null) _rooms.Remove(room);
                OnRoomRemoved(roomRes.Id);
            }
        }

        private void HandleBroadcast(object? source, string message)
        {
            if (!_connected) return;

            lock (_roomsLock)
            {
                if (XmlSerializerHelper.TryDeserialize<RoomBroadcast>(message, out var roomRes))
                {
                    if (roomRes.Removed)
                    {
                        DoRemoveRoom(roomRes);
                        return;
                    }
                    
                    if (roomRes.Updated)
                    {
                        DoUpdateRoom(roomRes);
                        return;
                    }

                    DoAddRoom(roomRes);
                }
                else if (XmlSerializerHelper.TryDeserialize<HeaterBroadcast>(message, out var heaterRes))
                {
                    if (heaterRes.Removed)
                    {
                        DoRemoveHeater(heaterRes.RoomId, heaterRes.Id);
                        return;
                    }
                    
                    if (heaterRes.Updated)
                    {
                        DoUpdateHeater(heaterRes.RoomId, heaterRes.Id, heaterRes.X, heaterRes.Y, heaterRes.Temperature, heaterRes.IsOn);
                        return;
                    }

                    DoAddHeater(heaterRes.RoomId, heaterRes.Id, heaterRes.X, heaterRes.Y, heaterRes.Temperature, heaterRes.IsOn);
                }
                else if (XmlSerializerHelper.TryDeserialize<HeatSensorBroadcast>(message, out var sensorRes))
                {
                    if (sensorRes.Removed)
                    {
                        DoRemoveHeatSensor(sensorRes.RoomId, sensorRes.Id);
                        return;
                    }
                    
                    if (sensorRes.Updated)
                    {
                        DoUpdateHeatSensor(sensorRes.RoomId, sensorRes.Id, sensorRes.X, sensorRes.Y, sensorRes.Temperature);
                        return;
                    }

                    DoAddHeatSensor(sensorRes.RoomId, sensorRes.Id, sensorRes.X, sensorRes.Y, sensorRes.Temperature);
                }
            }
        }

        private void OnHeaterAdded(Guid roomId, IHeaterData heater)
        {
            HeaterAdded?.Invoke(this, roomId, heater);
        }

        private void OnHeaterRemoved(Guid roomId, Guid heaterId)
        {
            HeaterRemoved?.Invoke(this, roomId, heaterId);
        }

        private void OnHeatSensorAdded(Guid roomId, IHeatSensorData sensor)
        {
            HeatSensorAdded?.Invoke(this, roomId, sensor);
        }

        private void OnHeatSensorRemoved(Guid roomId, Guid sensorId)
        {
            HeatSensorRemoved?.Invoke(this, roomId, sensorId);
        }

        private void OnRoomAdded(IRoomData room)
        {
            RoomAdded?.Invoke(this, room);
        }

        private void OnRoomRemoved(Guid roomId)
        {
            RoomRemoved?.Invoke(this, roomId);
        }

        private void OnClientConnected(object? source)
        {
            _connected = true;
            ClearAllData();
            GetAllData();
            ClientConnected?.Invoke(source);
        }

        public override void Dispose()
        {
            _client.ClientConnected -= OnClientConnected;
            _client.MessageReceived -= HandleBroadcast;
            _ = _client.DisconnectAsync();
            _rooms.Clear();

            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}