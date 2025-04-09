using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    public abstract class DataApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoomData> Rooms { get; }
        
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
        private readonly WebSocketClient _client = new();
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

        public event RoomChangedEventHandler? RoomChanged;
        public event HeaterChangedEventHandler? HeaterChanged;
        public event HeatSensorChangedEventHandler? HeatSensorChanged;

        public DataApi(string serverUri)
        {
            _client.ConnectAsync(serverUri).Wait();
            _client.MessageReceived += HandleBroadcast;
        }

        public override IHeaterData? AddHeater(Guid roomId, float x, float y, float temperature)
        {
            var room = GetRoom(roomId);
            if (room == null) return null;

            lock (_roomsLock)
            {
                var req = new AddHeaterRequest { RoomId = roomId, X = x, Y = y, Temperature = temperature };
                var xml = XmlSerializerHelper.Serialize(req);
                var responseXml = _client.SendAndReceiveAsync(xml).Result;
                var res = XmlSerializerHelper.Deserialize<HeaterAddedResponse>(responseXml);

                if (!res.Success) return null;

                IHeaterData heater = new HeaterData(res.Id, new PositionData(x, y), temperature);
                return room.AddHeater(heater);
            }
        }

        public override IHeaterData? GetHeater(Guid roomId, Guid id)
        {
            var room = GetRoom(roomId);
            if (room == null) return null;

            lock (_roomsLock)
            {
                var heater = room.GetHeater(id);
                if (heater != null) return heater;

                var req = new HeaterDataRequest { Id = id, RoomId = id };
                var xml = XmlSerializerHelper.Serialize(req);
                var responseXml = _client.SendAndReceiveAsync(xml).Result;

                var res = XmlSerializerHelper.Deserialize<HeaterDataRequestResponse>(responseXml);
                if (res.NotFound) return null;

                heater = new HeaterData(res.Id, new PositionData(res.X, res.Y), res.Temperature, res.IsOn);
                room.AddHeater(heater);
                return heater;
            }
        }

        public override void UpdateHeater(Guid roomId, Guid id, float x, float y, float temperature, bool isOn)
        {
            var room = GetRoom(roomId);
            if (room == null) return;

            lock (_roomsLock)
            {
                var req = new UpdateHeaterRequest { Id = id, RoomId = roomId, X = x, Y = y, Temperature = temperature, IsOn = isOn };
                var xml = XmlSerializerHelper.Serialize(req);
                var responseXml = _client.SendAndReceiveAsync(xml).Result;
                var res = XmlSerializerHelper.Deserialize<HeaterUpdatedResponse>(responseXml);

                if (!res.Success) return;

                var heater = room.GetHeater(id);

                if (heater == null) return;

                heater.Position.X = x;
                heater.Position.Y = y;
                heater.Temperature = temperature;
                heater.IsOn = isOn;
            }
        }

        public override void RemoveHeater(Guid roomId, Guid id)
        {
            var room = GetRoom(roomId);
            if (room == null) return;

            lock (_roomsLock)
            {
                var req = new RemoveHeaterRequest { Id = id, RoomId = roomId };
                var xml = XmlSerializerHelper.Serialize(req);
                var responseXml = _client.SendAndReceiveAsync(xml).Result;
                var res = XmlSerializerHelper.Deserialize<HeaterRemovedResponse>(responseXml);

                if (!res.Success) return;

                room.RemoveHeater(id);
            }
        }

        public override IHeatSensorData? AddHeatSensor(Guid roomId, float x, float y)
        {
            var room = GetRoom(roomId);
            if (room == null) return null;

            lock (_roomsLock)
            {
                var req = new AddHeatSensorRequest { RoomId = roomId, X = x, Y = y };
                var xml = XmlSerializerHelper.Serialize(req);
                var responseXml = _client.SendAndReceiveAsync(xml).Result;
                var res = XmlSerializerHelper.Deserialize<HeatSensorAddedResponse>(responseXml);

                if (!res.Success) return null;

                IHeatSensorData sensor = new HeatSensorData(res.Id, new PositionData(x, y));
                return room.AddHeatSensor(sensor);
            }
        }

        public override IHeatSensorData? GetHeatSensor(Guid roomId, Guid id)
        {
            var room = GetRoom(roomId);
            if (room == null) return null;

            lock (_roomsLock)
            {
                var sensor = room.GetHeatSensor(id);
                if (sensor != null) return sensor;

                var req = new HeatSensorDataRequest { Id = id, RoomId = id };
                var xml = XmlSerializerHelper.Serialize(req);
                var responseXml = _client.SendAndReceiveAsync(xml).Result;

                var res = XmlSerializerHelper.Deserialize<HeatSensorDataRequestResponse>(responseXml);
                if (res.NotFound) return null;

                sensor = new HeatSensorData(res.Id, new PositionData(res.X, res.Y), res.Temperature);
                room.AddHeatSensor(sensor);
                return sensor;
            }
        }

        public override void UpdateHeatSensor(Guid roomId, Guid id, float x, float y)
        {
            var room = GetRoom(roomId);
            if (room == null) return;

            lock (_roomsLock)
            {
                var req = new UpdateHeatSensorRequest { Id = id, RoomId = roomId, X = x, Y = y };
                var xml = XmlSerializerHelper.Serialize(req);
                var responseXml = _client.SendAndReceiveAsync(xml).Result;
                var res = XmlSerializerHelper.Deserialize<HeatSensorUpdatedResponse>(responseXml);

                if (!res.Success) return;

                var sensor = room.GetHeatSensor(id);

                if (sensor == null) return;

                sensor.Position.X = x;
                sensor.Position.Y = y;
            }
        }

        public override void RemoveHeatSensor(Guid roomId, Guid id)
        {
            var room = GetRoom(roomId);
            if (room == null) return;

            lock (_roomsLock)
            {
                var req = new RemoveHeatSensorRequest { Id = id, RoomId = roomId };
                var xml = XmlSerializerHelper.Serialize(req);
                var responseXml = _client.SendAndReceiveAsync(xml).Result;
                var res = XmlSerializerHelper.Deserialize<HeatSensorRemovedResponse>(responseXml);

                if (!res.Success) return;

                room.RemoveHeatSensor(id);
            }
        }

        public override IRoomData? AddRoom(string name, float width, float height)
        {
            lock (_roomsLock)
            {
                var req = new AddRoomRequest { Name = name, Width = width, Height = height };
                var xml = XmlSerializerHelper.Serialize(req);
                var responseXml = _client.SendAndReceiveAsync(xml).Result;
                var res = XmlSerializerHelper.Deserialize<RoomAddedResponse>(responseXml);

                if (!res.Success) return null;

                var room = new RoomData(res.Id, name, width, height);
                _rooms.Add(room);
                return room;
            }
        }

        public override IRoomData? GetRoom(Guid id)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(r => r.Id == id);
                if (room != null) return room;

                var req = new RoomDataRequest { RoomId = id };
                var xml = XmlSerializerHelper.Serialize(req);
                var responseXml = _client.SendAndReceiveAsync(xml).Result;

                var res = XmlSerializerHelper.Deserialize<RoomDataRequestResponse>(responseXml);
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
                return room;
            }
        }

        public override void UpdateRoom(Guid id, string name, float width, float height)
        {
            var req = new UpdateRoomRequest { Id = id, Name = name, Width = width, Height = height };
            var xml = XmlSerializerHelper.Serialize(req);
            var responseXml = _client.SendAndReceiveAsync(xml).Result;
            var res = XmlSerializerHelper.Deserialize<RoomUpdatedResponse>(responseXml);

            if (!res.Success) return;

            var room = GetRoom(id);
            if (room == null) return;

            lock (_roomsLock)
            {
                room.Name = name;
                room.Width = width;
                room.Height = height;
            }
        }

        public override void RemoveRoom(Guid id)
        {
            lock (_roomsLock)
            {
                var req = new RemoveRoomRequest { Id = id };
                var xml = XmlSerializerHelper.Serialize(req);
                var responseXml = _client.SendAndReceiveAsync(xml).Result;
                var res = XmlSerializerHelper.Deserialize<RoomRemovedResponse>(responseXml);

                if (!res.Success) return;

                var room = _rooms.Find(room => room.Id == id);
                if (room != null) _rooms.Remove(room);
            }
        }

        public override void Dispose()
        {
            _ = _client.DisconnectAsync();
            foreach (var room in _rooms)
            {
                room.Dispose();
            }
            _rooms.Clear();
            GC.SuppressFinalize(this);
        }

        private void HandleBroadcast(object? source, MessageReceivedEventArgs e)
        {
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
                            }

                            foreach (var sensorDto in roomRes.HeatSensors)
                            {
                                IHeatSensorData sensor = new HeatSensorData(
                                    sensorDto.Id,
                                    new PositionData(sensorDto.X, sensorDto.Y),
                                    sensorDto.Temperature
                                );
                                room.AddHeatSensor(sensor);
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
                                }
                                else
                                {
                                    heater.Position.X = heaterDto.X;
                                    heater.Position.Y = heaterDto.Y;
                                    heater.Temperature = heaterDto.Temperature;
                                    heater.IsOn = heaterDto.IsOn;
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
                                }
                                else
                                {
                                    sensor.Position.X = sensorDto.X;
                                    sensor.Position.Y = sensorDto.Y;
                                    sensor.Temperature = sensorDto.Temperature;
                                }
                            }
                        }
                    }
                    else
                    {
                        var room = new RoomData(roomRes.Id, roomRes.Name, roomRes.Width, roomRes.Height);
                        _rooms.Add(room);
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
                        var heater = new HeaterData(heaterRes.Id, new PositionData(heaterRes.X, heaterRes.Y), heaterRes.Temperature, heaterRes.IsOn);
                        room.AddHeater(heater);
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
                        var sensor = new HeatSensorData(sensorRes.Id, new PositionData(sensorRes.X, sensorRes.Y), sensorRes.Temperature);
                        room.AddHeatSensor(sensor);
                    }
                    OnHeatSensorChanged(this, sensorRes.Id, sensorRes.RoomId, sensorRes.Updated, sensorRes.Removed);
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
    }
}