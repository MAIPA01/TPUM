using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    public abstract class DataApiBase : IDisposable, INotifyRoomChanged, INotifyHeaterChanged, INotifyHeatSensorChanged, INotifyClientConnected
    {
        public abstract IReadOnlyCollection<IRoomData> Rooms { get; }

        public abstract event RoomChangedEventHandler? RoomChanged;
        public abstract event HeatSensorChangedEventHandler? HeatSensorChanged;
        public abstract event HeaterChangedEventHandler? HeaterChanged;
        public abstract event ClientConnectedEventHandler? ClientConnected;

        public abstract IHeaterData? AddHeater(Guid roomId, float x, float y, float temperature);

        public abstract IHeaterData? GetHeater(Guid roomId, Guid id);

        public abstract void UpdateHeater(Guid roomId, Guid id, float x, float y, float temperature, bool isOn);

        public abstract void RemoveHeater(Guid roomId, Guid id);

        public abstract IHeatSensorData? AddHeatSensor(Guid roomId, float x, float y);

        public abstract IHeatSensorData? GetHeatSensor(Guid roomId, Guid id);

        public abstract void UpdateHeatSensor(Guid roomId, Guid id, float x, float y);

        public abstract void RemoveHeatSensor(Guid roomId, Guid id);

        public abstract IRoomData? AddRoom(string name, float width, float height);

        public abstract IRoomData? GetRoom(Guid id);

        public abstract void UpdateRoom(Guid id, string name, float width, float height);

        public abstract void RemoveRoom(Guid id);

        public abstract void Dispose();

        public static DataApiBase GetApi(string serverUri)
        {
            return new DataApi(serverUri);
        }
    }

    internal class DataApi : DataApiBase
    {
        private readonly IWebSocketClient _client;
        private bool _connected = false;
        private readonly object _roomsLock = new();
        private readonly List<IRoomData> _rooms = [];

        public override event RoomChangedEventHandler? RoomChanged;
        public override event HeatSensorChangedEventHandler? HeatSensorChanged;
        public override event HeaterChangedEventHandler? HeaterChanged;
        public override event ClientConnectedEventHandler? ClientConnected;

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
            _client.ConnectAsync(serverUri);
            _client.MessageReceived += HandleBroadcast;
        }

        private TResponse SendRequestAndDeserialize<TRequest, TResponse>(TRequest request)
        {
            var xml = XmlSerializerHelper.Serialize<TRequest>(request);
            var responseXml = _client.SendAndReceiveAsync(xml).Result;
            return XmlSerializerHelper.Deserialize<TResponse>(responseXml);
        }

        public override IHeaterData? AddHeater(Guid roomId, float x, float y, float temperature)
        {
            if (!_connected) return null;

            var room = GetRoom(roomId);
            if (room == null) return null;

            lock (_roomsLock)
            {
                var req = new AddHeaterRequest { RoomId = roomId, X = x, Y = y, Temperature = temperature };
                var res = SendRequestAndDeserialize<AddHeaterRequest, HeaterAddedResponse>(req);

                if (!res.Success) return null;

                IHeaterData heater = new HeaterData(res.Id, new PositionData(x, y), temperature);
                heater = room.AddHeater(heater);
                OnHeaterChanged(this, res.Id, roomId, false, false);
                return heater;
            }
        }

        public override IHeaterData? GetHeater(Guid roomId, Guid id)
        {
            if (!_connected) return null;

            var room = GetRoom(roomId);
            if (room == null) return null;

            lock (_roomsLock)
            {
                var heater = room.GetHeater(id);
                if (heater != null) return heater;

                var req = new HeaterDataRequest { Id = id, RoomId = id };
                var res = SendRequestAndDeserialize<HeaterDataRequest, HeaterDataRequestResponse>(req);

                if (res.NotFound) return null;

                heater = new HeaterData(res.Id, new PositionData(res.X, res.Y), res.Temperature, res.IsOn);
                room.AddHeater(heater);
                OnHeaterChanged(this, id, roomId, false, false);
                return heater;
            }
        }

        public override void UpdateHeater(Guid roomId, Guid id, float x, float y, float temperature, bool isOn)
        {
            if (!_connected) return;

            var room = GetRoom(roomId);
            if (room == null) return;

            lock (_roomsLock)
            {
                var req = new UpdateHeaterRequest { Id = id, RoomId = roomId, X = x, Y = y, Temperature = temperature, IsOn = isOn };
                var res = SendRequestAndDeserialize<UpdateHeaterRequest, HeaterUpdatedResponse>(req);

                if (!res.Success) return;

                var heater = room.GetHeater(id);

                if (heater == null) return;

                heater.Position.X = x;
                heater.Position.Y = y;
                heater.Temperature = temperature;
                heater.IsOn = isOn;
            }

            OnHeaterChanged(this, id, roomId, true, false);
        }

        public override void RemoveHeater(Guid roomId, Guid id)
        {
            if (!_connected) return;

            var room = GetRoom(roomId);
            if (room == null) return;

            lock (_roomsLock)
            {
                var req = new RemoveHeaterRequest { Id = id, RoomId = roomId };
                var res = SendRequestAndDeserialize<RemoveHeaterRequest, HeaterRemovedResponse>(req);

                if (!res.Success) return;

                room.RemoveHeater(id);
            }

            OnHeaterChanged(this, id, roomId, false, true);
        }

        public override IHeatSensorData? AddHeatSensor(Guid roomId, float x, float y)
        {
            if (!_connected) return null;

            var room = GetRoom(roomId);
            if (room == null) return null;

            lock (_roomsLock)
            {
                var req = new AddHeatSensorRequest { RoomId = roomId, X = x, Y = y };
                var res = SendRequestAndDeserialize<AddHeatSensorRequest, HeatSensorAddedResponse>(req);

                if (!res.Success) return null;

                IHeatSensorData sensor = new HeatSensorData(res.Id, new PositionData(x, y));
                sensor = room.AddHeatSensor(sensor);
                OnHeatSensorChanged(this, res.Id, roomId, false, false);
                return sensor;
            }
        }

        public override IHeatSensorData? GetHeatSensor(Guid roomId, Guid id)
        {
            if (!_connected) return null;

            var room = GetRoom(roomId);
            if (room == null) return null;

            lock (_roomsLock)
            {
                var sensor = room.GetHeatSensor(id);
                if (sensor != null) return sensor;

                var req = new HeatSensorDataRequest { Id = id, RoomId = roomId };
                var res = SendRequestAndDeserialize<HeatSensorDataRequest, HeatSensorDataRequestResponse>(req);

                if (res.NotFound) return null;

                sensor = new HeatSensorData(res.Id, new PositionData(res.X, res.Y), res.Temperature);
                room.AddHeatSensor(sensor);
                OnHeatSensorChanged(this, id, roomId, false, false);
                return sensor;
            }
        }

        public override void UpdateHeatSensor(Guid roomId, Guid id, float x, float y)
        {
            if (!_connected) return;

            var room = GetRoom(roomId);
            if (room == null) return;

            lock (_roomsLock)
            {
                var req = new UpdateHeatSensorRequest { Id = id, RoomId = roomId, X = x, Y = y };
                var res = SendRequestAndDeserialize<UpdateHeatSensorRequest, HeatSensorUpdatedResponse>(req);

                if (!res.Success) return;

                var sensor = room.GetHeatSensor(id);

                if (sensor == null) return;

                sensor.Position.X = x;
                sensor.Position.Y = y;
            }

            OnHeatSensorChanged(this, id, roomId, true, false);
        }

        public override void RemoveHeatSensor(Guid roomId, Guid id)
        {
            if (!_connected) return;

            var room = GetRoom(roomId);
            if (room == null) return;

            lock (_roomsLock)
            {
                var req = new RemoveHeatSensorRequest { Id = id, RoomId = roomId };
                var res = SendRequestAndDeserialize<RemoveHeatSensorRequest, HeatSensorRemovedResponse>(req);

                if (!res.Success) return;

                room.RemoveHeatSensor(id);
            }

            OnHeatSensorChanged(this, id, roomId, false, true);
        }

        public override IRoomData? AddRoom(string name, float width, float height)
        {
            if (!_connected) return null;

            lock (_roomsLock)
            {
                var req = new AddRoomRequest { Name = name, Width = width, Height = height };
                var res = SendRequestAndDeserialize<AddRoomRequest, RoomAddedResponse>(req);

                if (!res.Success) return null;

                var room = new RoomData(res.Id, name, width, height);
                _rooms.Add(room);
                OnRoomChanged(this, res.Id, false, false);
                return room;
            }
        }

        public override IRoomData? GetRoom(Guid id)
        {
            if (!_connected) return null;

            lock (_roomsLock)
            {
                var room = _rooms.Find(r => r.Id == id);
                if (room != null) return room;

                var req = new RoomDataRequest { RoomId = id };
                var res = SendRequestAndDeserialize<RoomDataRequest, RoomDataRequestResponse>(req);

                if (res.NotFound) return null;

                room = new RoomData(res.Id, res.Name, res.Width, res.Height);

                foreach (var heaterDto in res.Heaters)
                {
                    IHeaterData heater = new HeaterData(
                        heaterDto.Id,
                        new PositionData(heaterDto.X, heaterDto.Y),
                        heaterDto.Temperature,
                        heaterDto.IsOn
                    );
                    room.AddHeater(heater);
                }

                foreach (var sensorDto in res.HeatSensors)
                {
                    IHeatSensorData sensor = new HeatSensorData(
                        sensorDto.Id,
                        new PositionData(sensorDto.X, sensorDto.Y),
                        sensorDto.Temperature
                    );
                    room.AddHeatSensor(sensor);
                }

                _rooms.Add(room);
                OnRoomChanged(this, id, false, false);
                return room;
            }
        }

        public override void UpdateRoom(Guid id, string name, float width, float height)
        {
            if (!_connected) return;

            var req = new UpdateRoomRequest { Id = id, Name = name, Width = width, Height = height };
            var res = SendRequestAndDeserialize<UpdateRoomRequest, RoomUpdatedResponse>(req);

            if (!res.Success) return;

            var room = GetRoom(id);
            if (room == null) return;

            lock (_roomsLock)
            {
                room.Name = name;
                room.Width = width;
                room.Height = height;
            }

            OnRoomChanged(this, id, true, false);
        }

        public override void RemoveRoom(Guid id)
        {
            if (!_connected) return;

            lock (_roomsLock)
            {
                var req = new RemoveRoomRequest { Id = id };
                var res = SendRequestAndDeserialize<RemoveRoomRequest, RoomRemovedResponse>(req);

                if (!res.Success) return;

                var room = _rooms.Find(room => room.Id == id);
                if (room != null)
                {
                    _rooms.Remove(room);
                    OnRoomChanged(this, id, false, true);
                }
            }
        }

        public override void Dispose()
        {
            _client.ClientConnected -= OnClientConnected;
            _client.MessageReceived -= HandleBroadcast;
            _ = _client.DisconnectAsync();
            foreach (var room in _rooms)
            {
                room.Dispose();
            }
            _rooms.Clear();

            _client.Dispose();
            GC.SuppressFinalize(this);
        }

        private void HandleBroadcast(object? source, MessageReceivedEventArgs e)
        {
            if (!_connected) return;

            lock (_roomsLock)
            {
                if (XmlSerializerHelper.TryDeserialize<RoomBroadcast>(e.XmlMessage, out var roomRes))
                {
                    if (roomRes.Removed) 
                    {
                        IRoomData? room = _rooms.Find(room => room.Id == roomRes.Id);
                        if (room != null) _rooms.Remove(room);
                    }
                    else if (roomRes.Updated)
                    {
                        var room = _rooms.Find(room => room.Id == roomRes.Id);
                        if (room == null)
                        {
                            room = new RoomData(roomRes.Id, roomRes.Name, roomRes.Width, roomRes.Height);

                            foreach (var heaterDto in roomRes.Heaters)
                            {
                                IHeaterData heater = new HeaterData(
                                    heaterDto.Id,
                                    new PositionData(heaterDto.X, heaterDto.Y),
                                    heaterDto.Temperature,
                                    heaterDto.IsOn
                                );
                                room.AddHeater(heater);

                                OnHeaterChanged(this, heaterDto.Id, roomRes.Id, false, false);
                            }

                            foreach (var sensorDto in roomRes.HeatSensors)
                            {
                                IHeatSensorData sensor = new HeatSensorData(
                                    sensorDto.Id,
                                    new PositionData(sensorDto.X, sensorDto.Y),
                                    sensorDto.Temperature
                                );
                                room.AddHeatSensor(sensor);

                                OnHeatSensorChanged(this, sensorDto.Id, roomRes.Id, false, false);
                            }

                            _rooms.Add(room);
                        }
                        else
                        {
                            foreach (var heaterDto in roomRes.Heaters)
                            {
                                IHeaterData? heater = room.GetHeater(heaterDto.Id);
                                if (heater == null)
                                {
                                    heater = new HeaterData(
                                        heaterDto.Id,
                                        new PositionData(heaterDto.X, heaterDto.Y),
                                        heaterDto.Temperature,
                                        heaterDto.IsOn
                                    );
                                    room.AddHeater(heater);

                                    OnHeaterChanged(this, heaterDto.Id, roomRes.Id, false, false);
                                }
                                else
                                {
                                    heater.Position.X = heaterDto.X;
                                    heater.Position.Y = heaterDto.Y;
                                    heater.Temperature = heaterDto.Temperature;
                                    heater.IsOn = heaterDto.IsOn;

                                    OnHeaterChanged(this, heaterDto.Id, roomRes.Id, true, false);
                                }
                            }

                            foreach (var sensorDto in roomRes.HeatSensors)
                            {
                                IHeatSensorData? sensor = room.GetHeatSensor(sensorDto.Id);
                                if (sensor == null)
                                {
                                    sensor = new HeatSensorData(
                                        sensorDto.Id,
                                        new PositionData(sensorDto.X, sensorDto.Y),
                                        sensorDto.Temperature
                                    );
                                    room.AddHeatSensor(sensor);

                                    OnHeatSensorChanged(this, sensorDto.Id, roomRes.Id, false, false);
                                }
                                else
                                {
                                    sensor.Position.X = sensorDto.X;
                                    sensor.Position.Y = sensorDto.Y;
                                    sensor.Temperature = sensorDto.Temperature;

                                    OnHeatSensorChanged(this, sensorDto.Id, roomRes.Id, true, false);
                                }
                            }
                        }
                    }
                    else
                    {
                        var room = _rooms.Find(r => r.Id == roomRes.Id);
                        if (room != null)
                        {
                            room.Name = roomRes.Name;
                            room.Width = roomRes.Width;
                            room.Height = roomRes.Height;
                        }
                        else
                        {
                            room = new RoomData(roomRes.Id, roomRes.Name, roomRes.Width, roomRes.Height);
                            _rooms.Add(room);
                        }
                    }
                    OnRoomChanged(this, roomRes.Id, roomRes.Updated, roomRes.Removed);
                }
                else if (XmlSerializerHelper.TryDeserialize<HeaterBroadcast>(e.XmlMessage, out var heaterRes))
                {
                    var room = _rooms.Find(r => r.Id == heaterRes.RoomId);
                    if (room == null) return;

                    if (heaterRes.Removed)
                    {
                        room.RemoveHeater(heaterRes.Id);
                    }
                    else if (heaterRes.Updated)
                    {
                        var heater = room.GetHeater(heaterRes.Id);
                        if (heater != null)
                        {
                            heater.Position.X = heaterRes.X;
                            heater.Position.Y = heaterRes.Y;
                            heater.IsOn = heaterRes.IsOn;
                            heater.Temperature = heaterRes.Temperature;
                        }
                        else
                        {
                            heater = new HeaterData(heaterRes.Id, new PositionData(heaterRes.X, heaterRes.Y), heaterRes.Temperature, heaterRes.IsOn);
                            room.AddHeater(heater);
                        }
                    }
                    else
                    {
                        var heater = room.GetHeater(heaterRes.Id);
                        if (heater != null)
                        {
                            heater.Position.X = heaterRes.X;
                            heater.Position.Y = heaterRes.Y;
                            heater.Temperature = heaterRes.Temperature;
                            heater.IsOn = heaterRes.IsOn;
                        }
                        else
                        {
                            heater = new HeaterData(heaterRes.Id, new PositionData(heaterRes.X, heaterRes.Y), heaterRes.Temperature, heaterRes.IsOn);
                            room.AddHeater(heater);
                        }
                    }
                    OnHeaterChanged(this, heaterRes.Id, heaterRes.RoomId, heaterRes.Updated, heaterRes.Removed);
                }
                else if (XmlSerializerHelper.TryDeserialize<HeatSensorBroadcast>(e.XmlMessage, out var sensorRes))
                {
                    var room = _rooms.FirstOrDefault(r => r.Id == sensorRes.RoomId);
                    if (room == null) return;

                    if (sensorRes.Removed)
                    {
                        room.RemoveHeater(sensorRes.Id);
                    }
                    else if (sensorRes.Updated)
                    {
                        var sensor = room.GetHeatSensor(sensorRes.Id);
                        if (sensor != null)
                        {
                            sensor.Position.X = sensorRes.X;
                            sensor.Position.Y = sensorRes.Y;
                            sensor.Temperature = sensorRes.Temperature;
                        }
                        else
                        {
                            sensor = new HeatSensorData(sensorRes.Id, new PositionData(sensorRes.X, sensorRes.Y), sensorRes.Temperature);
                            room.AddHeatSensor(sensor);
                        }
                    }
                    else
                    {
                        var sensor = room.GetHeatSensor(sensorRes.Id);
                        if (sensor != null)
                        {
                            sensor.Position.X = sensorRes.X;
                            sensor.Position.Y = sensorRes.Y;
                            sensor.Temperature = sensorRes.Temperature;
                        }
                        else
                        {
                            sensor = new HeatSensorData(sensorRes.Id, new PositionData(sensorRes.X, sensorRes.Y), sensorRes.Temperature);
                            room.AddHeatSensor(sensor);
                        }
                    }
                    OnHeatSensorChanged(this, sensorRes.Id, sensorRes.RoomId, sensorRes.Updated, sensorRes.Removed);
                }
            }
        }

        private void ClearAllData()
        {
            lock (_roomsLock)
            {
                while (_rooms.Count > 0)
                {
                    var room = _rooms.ElementAt(0);
                    OnRoomChanged(this, room.Id, false, true);
                    _rooms.RemoveAt(0);
                }
            }
        }

        private void GetAllData()
        {
            lock (_roomsLock)
            {
                var req = new AllDataRequest { IWant = true };
                var res = SendRequestAndDeserialize<AllDataRequest, AllDataRequestResponse>(req);

                foreach (var roomDto in res.Rooms)
                {
                    var room = new RoomData(roomDto.Id, roomDto.Name, roomDto.Width, roomDto.Height);
                    _rooms.Add(room);

                    OnRoomChanged(this, roomDto.Id, false, false);

                    foreach (var heaterDto in roomDto.Heaters)
                    {
                        var heater = new HeaterData(
                                heaterDto.Id,
                                new PositionData(heaterDto.X, heaterDto.Y),
                                heaterDto.Temperature,
                                heaterDto.IsOn
                            );
                        room.AddHeater(heater);

                        OnHeaterChanged(this, heaterDto.Id, roomDto.Id, false, false);
                    }

                    foreach (var sensorDto in roomDto.HeatSensors)
                    {
                        var sensor = new HeatSensorData(
                            sensorDto.Id,
                            new PositionData(sensorDto.X, sensorDto.Y),
                            sensorDto.Temperature
                        );
                        room.AddHeatSensor(sensor);

                        OnHeatSensorChanged(this, sensorDto.Id, roomDto.Id, false, false);
                    }
                }
            }
        }

        private void OnRoomChanged(object? source, Guid id, bool updated, bool removed)
        {
            RoomChanged?.Invoke(source, new RoomChangedEventArgs(id, updated, removed));
        }

        private void OnHeaterChanged(object? source, Guid id, Guid roomId, bool updated, bool removed)
        {
            HeaterChanged?.Invoke(source, new HeaterChangedEventArgs(id, roomId, updated, removed));
        }

        private void OnHeatSensorChanged(object? source, Guid id, Guid roomId, bool updated, bool removed)
        {
            HeatSensorChanged?.Invoke(source, new HeatSensorChangedEventArgs(id, roomId, updated, removed));
        }

        private void OnClientConnected(object? source, ClientConnectedEventArgs e)
        {
            _connected = true;
            ClearAllData();
            GetAllData();
            ClientConnected?.Invoke(source, e);
        }
    }
}