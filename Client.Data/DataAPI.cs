using TPUM.Client.Data.Events;
using TPUM.XmlShared;
using TPUM.XmlShared.Request;
using TPUM.XmlShared.Response.Broadcast;
using TPUM.XmlShared.Response.Client;
using TPUM.XmlShared.Response.Subscribe;

namespace TPUM.Client.Data
{
    public abstract class DataApiBase : IDisposable, INotifyRoomRemoved, INotifyRoomAdded, INotifyClientConnected
    {
        public abstract IReadOnlyCollection<IRoomData> Rooms { get; }

        public abstract event ClientConnectedEventHandler? ClientConnected;
        public abstract event RoomRemovedEventHandler? RoomRemoved;
        public abstract event RoomAddedEventHandler? RoomAdded;

        public abstract void AddRoom(string name, float width, float height);

        public abstract bool ContainsRoom(Guid roomId);

        public abstract IRoomData? GetRoom(Guid roomId);

        public abstract void RemoveRoom(Guid roomId);

        public abstract void Refresh();

        public abstract void Dispose();

        private static DataApiBase? _instance = null;

        public static DataApiBase GetApi(string serverUri)
        {
            return _instance ??= new DataApi(serverUri);
        }
    }

    internal class DataApi : DataApiBase
    {
        private readonly WebSocketClient _client;
        private bool _connected = false;

        public override event ClientConnectedEventHandler? ClientConnected;
        public override event RoomRemovedEventHandler? RoomRemoved;
        public override event RoomAddedEventHandler? RoomAdded;

        private readonly object _roomsLock = new();
        private readonly List<RoomData> _rooms = [];
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
            _client.SubscribeReceived += HandleSubscribe;
            _client.BroadcastReceived += HandleBroadcast;
            _ = _client.ConnectAsync(serverUri);
        }

        private void SendRequest<TRequest>(TRequest request)
        {
            var xml = XmlSerializerHelper.Serialize(request);
            _ = _client.SendAsync(xml);
        }

        internal void SubscribeToRoomTemperature(Guid roomId)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateSubscribeRoomTemperatureRequest(roomId));
        }

        internal void UnsubscribeFromRoomTemperature(Guid roomId)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateUnsubscribeRoomTemperatureRequest(roomId));
        }

        internal void GetHeater(Guid roomId, Guid heaterId)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateGetHeaterDataRequest(roomId, heaterId));
        }

        internal void AddHeater(Guid roomId, float x, float y, float temperature)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateAddHeaterRequest(roomId, x, y, temperature));
        }

        internal void UpdateHeater(Guid roomId, Guid heaterId, float x, float y, float temperature, bool isOn)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateUpdateHeaterRequest(roomId, heaterId, x, y, temperature, isOn));
        }

        internal void RemoveHeater(Guid roomId, Guid heaterId)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateRemoveHeaterRequest(roomId, heaterId));
        }

        internal void GetHeatSensor(Guid roomId, Guid sensorId)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateGetHeatSensorDataRequest(roomId, sensorId));
        }

        internal void AddHeatSensor(Guid roomId, float x, float y)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateAddHeatSensorRequest(roomId, x, y));
        }

        internal void UpdateHeatSensor(Guid roomId, Guid sensorId, float x, float y)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateUpdateHeatSensorRequest(roomId, sensorId, x, y));
        }

        internal void RemoveHeatSensor(Guid roomId, Guid sensorId)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateRemoveHeatSensorRequest(roomId, sensorId));
        }

        public override void AddRoom(string name, float width, float height)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateAddRoomRequest(name, width, height));
        }

        public override bool ContainsRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                return _rooms.Any(room => room.Id == roomId);
            }
        }

        public override IRoomData? GetRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == roomId);
                if (room != null) return room;

                if (!_connected) return null;

                SendRequest(XmlRequestFactory.CreateGetRoomDataRequest(roomId));

                return null;
            }
        }

        public override void RemoveRoom(Guid roomId)
        {
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateRemoveRoomRequest(roomId));
        }

        public override void Refresh()
        {
            ClearAllData();
            GetAllData();
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
            if (!_connected) return;

            SendRequest(XmlRequestFactory.CreateGetAllDataRequest());
        }

        private void DoGetAllData(List<RoomDataContract> rooms)
        {
            lock (_roomsLock)
            {
                // Uznajemy ¿e nie ma niczego
                foreach (var roomData in rooms)
                {
                    var room = new RoomData(
                        this, 
                        roomData.RoomId,
                        roomData.Name,
                        roomData.Width,
                        roomData.Height
                    );

                    _rooms.Add(room);
                    OnRoomAdded(room);

                    foreach (var heaterData in roomData.Heaters)
                    {
                        var heater = new HeaterData(
                            this,
                            roomData.RoomId,
                            heaterData.HeaterId,
                            new PositionData(heaterData.X, heaterData.Y),
                            heaterData.Temperature,
                            heaterData.IsOn
                        );
                        room.OnHeaterAdded(heater);
                    }

                    foreach (var sensorData in roomData.HeatSensors)
                    {
                        var sensor = new HeatSensorData(
                            this,
                            roomData.RoomId,
                            sensorData.HeatSensorId,
                            new PositionData(sensorData.X, sensorData.Y), 
                            sensorData.Temperature
                        );
                        room.OnHeatSensorAdded(sensor);
                    }
                }
            }
        }

        private void DoGetHeater(Guid roomId, Guid heaterId, GetHeaterClientDataResult heaterData)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == roomId);
                if (room == null)
                {
                    GetRoom(roomId);
                    return;
                }
                
                // Wywo³uje siê tylko gdy nie ma lokalnej kopi
                var heater = new HeaterData(
                    this,
                    roomId,
                    heaterId,
                    new PositionData(heaterData.X, heaterData.Y),
                    heaterData.Temperature,
                    heaterData.IsOn
                );
                room.OnHeaterAdded(heater);
            }
        }

        private void DoAddHeater(AddHeaterBroadcastData heaterData)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == heaterData.RoomId);
                if (room == null) return;

                if (room.ContainsHeater(heaterData.HeaterId)) return;

                var heater = new HeaterData(
                    this,
                    heaterData.RoomId,
                    heaterData.HeaterId,
                    new PositionData(heaterData.X, heaterData.Y),
                    heaterData.Temperature
                );
                room.OnHeaterAdded(heater);
            }
        }

        private void DoUpdateHeater(UpdateHeaterBroadcastData heaterData)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == heaterData.RoomId);
                if (room == null) return;

                var heater = room.GetHeater(heaterData.HeaterId);
                if (heater == null)
                {
                    // add heater
                    heater = new HeaterData(
                        this,
                        heaterData.RoomId,
                        heaterData.HeaterId,
                        new PositionData(heaterData.X, heaterData.Y),
                        heaterData.Temperature,
                        heaterData.IsOn
                    );
                    room.OnHeaterAdded(heater);
                    return;
                }

                (heater as HeaterData)?.UpdateDataFromServer(heaterData.X, heaterData.Y, heaterData.Temperature, heaterData.IsOn);
            }
        }

        private void DoRemoveHeater(RemoveHeaterBroadcastData heaterData)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == heaterData.RoomId);
                if (room == null) return;

                room.OnHeaterRemoved(heaterData.HeaterId);
            }
        }

        private void DoGetHeatSensor(Guid roomId, Guid sensorId, GetHeatSensorClientDataResult sensorData)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == roomId);
                if (room == null)
                {
                    GetRoom(roomId);
                    return;
                }

                // Wywo³uje siê tylko gdy nie ma lokalnej kopi
                var sensor = new HeatSensorData(
                    this,
                    roomId,
                    sensorId,
                    new PositionData(sensorData.X, sensorData.Y),
                    sensorData.Temperature
                );
                room.OnHeatSensorAdded(sensor);
            }
        }

        private void DoAddHeatSensor(AddHeatSensorBroadcastData sensorData)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == sensorData.RoomId);
                if (room == null) return;

                if (room.ContainsHeatSensor(sensorData.HeatSensorId)) return;

                var sensor = new HeatSensorData(
                    this,
                    sensorData.RoomId,
                    sensorData.HeatSensorId,
                    new PositionData(sensorData.X, sensorData.Y),
                    sensorData.Temperature
                );
                room.OnHeatSensorAdded(sensor);
            }
        }

        private void DoUpdateHeatSensor(UpdateHeatSensorBroadcastData sensorData)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == sensorData.RoomId);
                if (room == null) return;

                var sensor = room.GetHeatSensor(sensorData.HeatSensorId);
                if (sensor == null)
                {
                    sensor = new HeatSensorData(
                        this,
                        sensorData.RoomId,
                        sensorData.HeatSensorId,
                        new PositionData(sensorData.X, sensorData.Y),
                        sensorData.Temperature
                    );
                    room.OnHeatSensorAdded(sensor);
                    return;
                }

                (sensor as HeatSensorData)?.UpdateDataFromServer(sensorData.X, sensorData.Y, sensorData.Temperature);
            }
        }

        private void DoRemoveHeatSensor(RemoveHeatSensorBroadcastData sensorData)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == sensorData.RoomId);
                if (room == null) return;

                room.OnHeatSensorRemoved(sensorData.HeatSensorId);
            }
        }

        private void DoGetRoom(Guid roomId, GetRoomClientDataResult roomData)
        {
            lock (_roomsLock)
            {
                // Wywo³uje siê tylko gdy nie ma lokalnej kopi
                var room = new RoomData(
                    this,
                    roomId,
                    roomData.Name,
                    roomData.Width,
                    roomData.Height
                );
                _rooms.Add(room);
                OnRoomAdded(room);

                foreach (var heaterData in roomData.Heaters)
                {
                    var heater = new HeaterData(
                        this,
                        roomId,
                        heaterData.HeaterId,
                        new PositionData(heaterData.X, heaterData.Y),
                        heaterData.Temperature,
                        heaterData.IsOn
                    );
                    room.OnHeaterAdded(heater);
                }

                foreach (var sensorData in roomData.HeatSensors)
                {
                    var sensor = new HeatSensorData(
                        this,
                        roomId,
                        sensorData.HeatSensorId,
                        new PositionData(sensorData.X, sensorData.Y),
                        sensorData.Temperature
                    );
                    room.OnHeatSensorAdded(sensor);
                }
            }
        }

        private void DoAddRoom(AddRoomBroadcastData roomData)
        {
            lock (_roomsLock)
            {
                if (_rooms.Any(room => room.Id == roomData.RoomId)) return;

                var room = new RoomData(this, roomData.RoomId, roomData.Name, roomData.Width, roomData.Height);

                _rooms.Add(room);
                OnRoomAdded(room);
            }
        }

        private void DoRemoveRoom(RemoveRoomBroadcastData roomData)
        {
            lock (_roomsLock)
            {
                var room = _rooms.Find(room => room.Id == roomData.RoomId);
                if (room != null) _rooms.Remove(room);
                OnRoomRemoved(roomData.RoomId);
            }
        }

        private void HandleSubscribe(object? source, SubscribeResponseContent subscribe)
        {
            if (!_connected) return;

            lock (_roomsLock)
            {
                // room temperature
                if (subscribe.DataType == SubscribeResponseType.RoomTemperature)
                {
                    var roomTemperatureSubscribe = (RoomTemperatureSubscribeData)subscribe.Data;

                    var room = GetRoom(roomTemperatureSubscribe.RoomId);
                    var sensor = room?.GetHeatSensor(roomTemperatureSubscribe.HeatSensorId);
                    if (sensor == null) return;

                    (sensor as HeatSensorData)!.UpdateDataFromServer(
                        sensor.Position.X, sensor.Position.Y, roomTemperatureSubscribe.Temperature);
                }
            }
        }

        private void HandleResponse(object? source, ClientResponseContent response)
        {
            if (!_connected) return;

            lock (_roomsLock)
            {
                // get
                if (response.DataType == ClientResponseType.Get)
                {
                    var getResponse = (GetClientResponseData)response.Data;
                    // all
                    if (getResponse.DataType == GetClientType.All)
                    {
                        DoGetAllData(((GetAllClientData)getResponse.Data).Rooms);
                    }
                    // room
                    else if (getResponse.DataType == GetClientType.Room)
                    {
                        var getRoomResponse = (GetRoomClientData)getResponse.Data;
                        if (getRoomResponse.Success) DoGetRoom(getRoomResponse.RoomId, getRoomResponse.Result!);
                        else
                        {
                            // Room not found
                        }
                    }
                    // heater
                    else if (getResponse.DataType == GetClientType.Heater)
                    {
                        var getHeaterResponse = (GetHeaterClientData)getResponse.Data;
                        if (getHeaterResponse.Success) DoGetHeater(getHeaterResponse.RoomId, getHeaterResponse.HeaterId, getHeaterResponse.Result!);
                        else
                        {
                            // Heater not found
                        }
                    }
                    // heatSensor
                    else if (getResponse.DataType == GetClientType.HeatSensor)
                    {
                        var getHeatSensorResponse = (GetHeatSensorClientData)getResponse.Data;
                        if (getHeatSensorResponse.Success) DoGetHeatSensor(getHeatSensorResponse.RoomId, getHeatSensorResponse.HeatSensorId,
                            getHeatSensorResponse.Result!);
                        else
                        {
                            // Heat Sensor not found
                        }
                    }
                }
                // add
                else if (response.DataType == ClientResponseType.Add)
                {
                    var addResponse = (AddClientResponseData)response.Data;
                    // room
                    if (addResponse.DataType == AddClientType.Room)
                    {
                        var addRoomResponse = (AddRoomClientData)addResponse.Data!;
                        if (addResponse.Success)
                        {
                            // Successfully added room
                        }
                        else
                        {
                            // room not added
                        }
                    }
                    // heater
                    else if (addResponse.DataType == AddClientType.Heater)
                    {
                        var addHeaterResponse = (AddHeaterClientData)addResponse.Data!;
                        if (addResponse.Success)
                        {
                            // Successfully added heater
                        }
                        else
                        {
                            // heater not added
                        }
                    }
                    // heatSensor
                    else if (addResponse.DataType == AddClientType.HeatSensor)
                    {
                        var addHeatSensorResponse = (AddHeatSensorClientData)addResponse.Data!;
                        if (addResponse.Success)
                        {
                            // Successfully added heat sensor
                        }
                        else
                        {
                            // heat sensor not added
                        }
                    }
                }
                // update
                else if (response.DataType == ClientResponseType.Update)
                {
                    var updateResponse = (UpdateClientResponseData)response.Data;
                    // heater
                    if (updateResponse.DataType == UpdateClientType.Heater)
                    {
                        var updateHeaterResponse = (UpdateHeaterClientData)updateResponse.Data;
                        if (updateResponse.Success)
                        {
                            // Successfully updated heater
                        }
                        else
                        {
                            // update heater failed
                        }
                    }
                    // heatSensor
                    else if (updateResponse.DataType == UpdateClientType.HeatSensor)
                    {
                        var updateHeatSensorResponse = (UpdateHeatSensorClientData)updateResponse.Data;
                        if (updateResponse.Success)
                        {
                            // Successfully updated heat sensor
                        }
                        else
                        {
                            // update heat sensor failed
                        }
                    }
                }
                // remove
                else if (response.DataType == ClientResponseType.Remove)
                {
                    var removeResponse = (RemoveClientResponseData)response.Data;
                    // room
                    if (removeResponse.DataType == RemoveClientType.Room)
                    {
                        var removeRoomResponse = (RemoveRoomClientData)removeResponse.Data;
                        if (removeResponse.Success)
                        {
                            // Successfully removed room
                        }
                        else
                        {
                            // room removal failed
                        }
                    }
                    // heater
                    else if (removeResponse.DataType == RemoveClientType.Heater)
                    {
                        var removeHeaterResponse = (RemoveHeaterClientData)removeResponse.Data;
                        if (removeResponse.Success)
                        {
                            // Successfully removed heater
                        }
                        else
                        {
                            // heater removal failed
                        }
                    }
                    // heatSensor
                    else if (removeResponse.DataType == RemoveClientType.HeatSensor)
                    {
                        var removeHeatSensorResponse = (RemoveHeatSensorClientData)removeResponse.Data;
                        if (removeResponse.Success)
                        {
                            // Successfully removed heat sensor
                        }
                        else
                        {
                            // heat sensor removal failed
                        }
                    }
                }
                // subscribe
                else if (response.DataType == ClientResponseType.Subscribe)
                {
                    var subscribeResponse = (SubscribeClientResponseData)response.Data;
                    // room temperature
                    if (subscribeResponse.DataType == SubscribeClientType.RoomTemperature)
                    {
                        var roomTemperatureSubscribeResponse =
                            (SubscribeRoomTemperatureClientData)subscribeResponse.Data;
                        if (subscribeResponse.Success)
                        {
                            // Successfully subscribed
                        }
                        else
                        {
                            // Failed to subscribe
                        }
                    }
                }
                // unsubscribe
                else if (response.DataType == ClientResponseType.Unsubscribe)
                {
                    var unsubscribeResponse = (UnsubscribeClientResponseData)response.Data;
                    // room temperature
                    if (unsubscribeResponse.DataType == UnsubscribeClientType.RoomTemperature)
                    {
                        var roomTemperatureUnsubscribeResponse =
                            (UnsubscribeRoomTemperatureClientData)unsubscribeResponse.Data;
                        if (unsubscribeResponse.Success)
                        {
                            // Successfully unsubscribed
                        }
                        else
                        {
                            // Failed to unsubscribe
                        }
                    }
                }
            }
        }

        private void HandleBroadcast(object? source, BroadcastResponseContent broadcast)
        {
            if (!_connected) return;

            lock (_roomsLock)
            {
                // add
                if (broadcast.BroadcastType == BroadcastResponseType.Add)
                {
                    var addBroadcast = (AddBroadcastResponse)broadcast.Broadcast;
                    // room
                    if (addBroadcast.DataType == AddBroadcastType.Room)
                    {
                        DoAddRoom((AddRoomBroadcastData)addBroadcast.Data);
                    }
                    // heater
                    else if (addBroadcast.DataType == AddBroadcastType.Heater)
                    {
                        DoAddHeater((AddHeaterBroadcastData)addBroadcast.Data);
                    }
                    // heat sensor
                    else if (addBroadcast.DataType == AddBroadcastType.HeatSensor)
                    {
                        DoAddHeatSensor((AddHeatSensorBroadcastData)addBroadcast.Data);
                    }
                }
                // update
                else if (broadcast.BroadcastType == BroadcastResponseType.Update)
                {
                    var updateBroadcast = (UpdateBroadcastResponse)broadcast.Broadcast;
                    // heater
                    if (updateBroadcast.DataType == UpdateBroadcastType.Heater)
                    {
                        DoUpdateHeater((UpdateHeaterBroadcastData)updateBroadcast.Data);
                    }
                    // heat sensor
                    else if (updateBroadcast.DataType == UpdateBroadcastType.HeatSensor)
                    {
                        DoUpdateHeatSensor((UpdateHeatSensorBroadcastData)updateBroadcast.Data);
                    }
                }
                // remove
                else if (broadcast.BroadcastType == BroadcastResponseType.Remove)
                {
                    var removeBroadcast = (RemoveBroadcastResponse)broadcast.Broadcast;
                    // room
                    if (removeBroadcast.DataType == RemoveBroadcastType.Room)
                    {
                        DoRemoveRoom((RemoveRoomBroadcastData)removeBroadcast.Data);
                    }
                    // heater
                    else if (removeBroadcast.DataType == RemoveBroadcastType.Heater)
                    {
                        DoRemoveHeater((RemoveHeaterBroadcastData)removeBroadcast.Data);
                    }
                    // heat sensor
                    else if (removeBroadcast.DataType == RemoveBroadcastType.HeatSensor)
                    {
                        DoRemoveHeatSensor((RemoveHeatSensorBroadcastData)removeBroadcast.Data);
                    }
                }
            }
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
            Refresh();
            ClientConnected?.Invoke(source);
        }

        public override void Dispose()
        {
            _client.ClientConnected -= OnClientConnected;
            _client.ResponseReceived -= HandleResponse;
            _client.SubscribeReceived -= HandleSubscribe;
            _client.BroadcastReceived -= HandleBroadcast;
            _ = _client.DisconnectAsync();
            _rooms.Clear();

            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}