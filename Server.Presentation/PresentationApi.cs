using TPUM.Server.Logic;
using TPUM.XmlShared;
using TPUM.XmlShared.Generated;
using TPUM.XmlShared.Generated.Factory;

namespace TPUM.Server.Presentation
{
    public abstract class PresentationApiBase : IDisposable
    {
        public abstract Task StartServer();

        public abstract void Dispose();

        private static PresentationApiBase? _instance = null;

        public static PresentationApiBase GetApi(string uriPrefix, LogicApiBase? logic = null)
        {
            return _instance ??= new PresentationApi(uriPrefix, logic ?? LogicApiBase.GetApi());
        }
    }

    internal class PresentationApi : PresentationApiBase
    {
        private readonly LogicApiBase _logic;

        private readonly WebSocketServer _server;
        private readonly Dictionary<Guid, List<Guid>> _roomTemperatureSubscribers = []; // roomId -> List of ClientIds

        private readonly object _roomsLock = new();
        private readonly List<Room> _rooms = [];

        public PresentationApi(string uriPrefix, LogicApiBase logic)
        {
            _logic = logic;
            foreach (var logicRoom in _logic.Rooms)
            {
                var room = new Room(logicRoom);
                SubscribeToRoom(room);
                _rooms.Add(room);
            }
            _server = new WebSocketServer(uriPrefix);
            _server.ClientRequestReceived += HandleClientRequest;
            _server.ClientDisconnected += GetClientDisconnected;
        }

        public override Task StartServer()
        {
            return _server.StartAsync();
        }

        private void SendClientResponse<TResponse>(Guid clientId, TResponse response)
        {
            var message = XmlSerializerHelper.Serialize(response);
            _ = _server.SendAsync(clientId, message);
        }

        private void SendToClientsResponse<TResponse>(List<Guid> clientsIds, TResponse response)
        {
            var message = XmlSerializerHelper.Serialize(response);
            _ = _server.SendToClientsAsync(clientsIds, message);
        }

        private void SendBroadcastResponse<TResponse>(TResponse response)
        {
            var message = XmlSerializerHelper.Serialize(response);
            _ = _server.BroadcastAsync(message);
        }

        private void GetPositionChange(Guid roomId, object? source, IPosition lastPosition, IPosition newPosition)
        {
            switch (source)
            {
                case IHeater heater:
                    Console.WriteLine($"-> Position Changed in Heater [{heater.Id}] in Room [{roomId}]. Change Broadcasted to clients");
                    SendHeaterUpdatedBroadcast(roomId, heater.Id);
                    break;
                case IHeatSensor sensor:
                    Console.WriteLine($"-> Position Changed in Heat Sensor [{sensor.Id}] in Room [{roomId}]. Change Broadcasted to clients");
                    SendHeatSensorUpdatedBroadcast(roomId, sensor.Id);
                    break;
            }
        }

        private void GetTemperatureChange(Guid roomId, object? source, float lastTemperature, float newTemperature)
        {
            switch (source)
            {
                case IHeater heater:
                    Console.WriteLine($"-> Temperature Changed in Heater [{heater.Id}] in Room [{roomId}]. Change Broadcasted to clients");
                    SendHeaterUpdatedBroadcast(roomId, heater.Id);
                    break;
                case IHeatSensor sensor:
                    Console.WriteLine($"-> Temperature Changed in Heat Sensor [{sensor.Id}] in Room [{roomId}]. Change Broadcasted to subscribed clients");
                    SendHeatSensorUpdatedToSubscribers(roomId, sensor.Id);
                    break;
            }
        }

        private void GetEnableChange(Guid roomId, object? source, bool lastEnable, bool newEnable)
        {
            switch (source)
            {
                case IHeater heater:
                    Console.WriteLine($"-> Enable Changed in Heater [{heater.Id}] in Room [{roomId}]. Change Broadcasted to clients");
                    SendHeaterUpdatedBroadcast(roomId, heater.Id);
                    break;
            }
        }

        private void GetClientDisconnected(object? source, Guid clientId)
        {
            foreach (var (roomId, clientsIds) in _roomTemperatureSubscribers)
            {
                clientsIds.Remove(clientId);
            }
        }

        private void SubscribeToRoom(Room room)
        {
            room.PositionChanged += GetPositionChange;
            room.TemperatureChanged += GetTemperatureChange;
            room.EnableChanged += GetEnableChange;
        }

        private void UnsubscribeFromRoom(Room room)
        {
            room.PositionChanged -= GetPositionChange;
            room.TemperatureChanged -= GetTemperatureChange;
            room.EnableChanged -= GetEnableChange;
        }

        private Guid AddRoom(string name, float width, float height)
        {
            var room = new Room(_logic.AddRoom(name, width, height));
            SubscribeToRoom(room);
            lock (_roomsLock)
            {
                _rooms.Add(room);
            }
            return room.Id;
        }

        private bool ContainsRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                return _rooms.Any(room => room.Id == roomId);
            }
        }

        private Room? GetRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                return _rooms.Find(room => room.Id == roomId);
            }
        }

        private bool RemoveRoom(Guid roomId)
        {
            lock (_roomsLock)
            {
                var room = GetRoom(roomId);
                if (room != null)
                {
                    UnsubscribeFromRoom(room);
                    _rooms.Remove(room);
                    _roomTemperatureSubscribers.Remove(roomId);
                }
            }
            _logic.RemoveRoom(roomId);
            return true;
        }

        private void ClearRooms()
        {
            lock (_roomsLock)
            {
                foreach (var room in _rooms)
                {
                    UnsubscribeFromRoom(room);
                }
                _rooms.Clear();
            }
        }

        private void SendRoomAddedBroadcast(Guid roomId)
        {
            lock (_roomsLock)
            {
                var room = GetRoom(roomId);
                if (room == null) return;
                SendBroadcastResponse(XmlBroadcastResponseFactory.CreateAddRoomBroadcastResponse(roomId, room.Name, room.Width, room.Height));
            }
        }

        private void SendRoomRemovedBroadcast(Guid roomId)
        {
            lock (_roomsLock)
            {
                SendBroadcastResponse(XmlBroadcastResponseFactory.CreateRemoveRoomBroadcastResponse(roomId));
            }
        }

        private Guid AddHeater(Guid roomId, float x, float y, float temperature)
        {
            lock (_roomsLock)
            {
                var room = GetRoom(roomId);
                if (room == null) return Guid.Empty;

                var heater = room.AddHeater(x, y, temperature);
                return heater.Id;
            }
        }

        private bool UpdateHeater(Guid roomId, Guid heaterId, float x, float y, float temperature, bool isOn)
        {
            lock (_roomsLock)
            {
                var room = GetRoom(roomId);

                var heater = room?.GetHeater(heaterId);
                if (heater == null) return false;

                heater.SetPosition(x, y);
                heater.Temperature = temperature;
                if (isOn)
                {
                    heater.TurnOn();
                }
                else
                {
                    heater.TurnOff();
                }

                return true;
            }
        }

        private IHeater? GetHeater(Guid roomId, Guid heaterId)
        {
            lock (_roomsLock)
            {
                var room = GetRoom(roomId);
                return room?.GetHeater(heaterId);
            }
        }

        private bool RemoveHeater(Guid roomId, Guid heaterId)
        {
            lock (_roomsLock)
            {
                var room = GetRoom(roomId);
                room?.RemoveHeater(heaterId);
                return true;
            }
        }

        private void SendHeaterAddedBroadcast(Guid roomId, Guid heaterId)
        {
            lock (_roomsLock)
            {
                var heater = GetHeater(roomId, heaterId);
                if (heater == null) return;
                SendBroadcastResponse(XmlBroadcastResponseFactory.CreateAddHeaterBroadcastResponse(roomId, heaterId, heater.Position.X, 
                    heater.Position.Y, heater.Temperature));
            }
        }

        private void SendHeaterUpdatedBroadcast(Guid roomId, Guid heaterId)
        {
            lock (_roomsLock)
            {
                var heater = GetHeater(roomId, heaterId);
                if (heater == null) return;
                SendBroadcastResponse(XmlBroadcastResponseFactory.CreateUpdateHeaterBroadcastResponse(roomId, heaterId, 
                    heater.Position.X, heater.Position.Y, heater.Temperature, heater.IsOn));
            }
        }

        private void SendHeaterRemovedBroadcast(Guid roomId, Guid heaterId)
        {
            lock (_roomsLock)
            {
                SendBroadcastResponse(XmlBroadcastResponseFactory.CreateRemoveHeaterBroadcastResponse(roomId, heaterId));
            }
        }

        private Guid AddHeatSensor(Guid roomId, float x, float y)
        {
            lock (_roomsLock)
            {
                var room = GetRoom(roomId);
                if (room == null) return Guid.Empty;

                var sensor = room.AddHeatSensor(x, y);
                return sensor.Id;
            }
        }

        private bool UpdateHeatSensor(Guid roomId, Guid sensorId, float x, float y)
        {
            lock (_roomsLock)
            {
                var room = GetRoom(roomId);
                var sensor = room?.GetHeatSensor(sensorId);
                if (sensor == null) return false;

                sensor.SetPosition(x, y);
                return true;
            }
        }

        private IHeatSensor? GetHeatSensor(Guid roomId, Guid sensorId)
        {
            lock (_roomsLock)
            {
                var room = GetRoom(roomId);
                return room?.GetHeatSensor(sensorId);
            }
        }

        private bool RemoveHeatSensor(Guid roomId, Guid sensorId)
        {
            lock (_roomsLock)
            {
                var room = GetRoom(roomId);
                room?.RemoveHeatSensor(sensorId);
                return true;
            }
        }

        private void SendHeatSensorAddedBroadcast(Guid roomId, Guid sensorId)
        {
            lock (_roomsLock)
            {
                var sensor = GetHeatSensor(roomId, sensorId);
                if (sensor == null) return;
                SendBroadcastResponse(XmlBroadcastResponseFactory.CreateAddHeatSensorBroadcastResponse(roomId, sensorId, 
                    sensor.Position.X, sensor.Position.Y, sensor.Temperature));
            }
        }

        private void SendHeatSensorUpdatedBroadcast(Guid roomId, Guid sensorId)
        {
            lock (_roomsLock)
            {
                var sensor = GetHeatSensor(roomId, sensorId);
                if (sensor == null) return;
                SendBroadcastResponse(XmlBroadcastResponseFactory.CreateUpdateHeatSensorBroadcastResponse(roomId, sensorId, 
                    sensor.Position.X, sensor.Position.Y, sensor.Temperature));
            }
        }

        private void SendHeatSensorUpdatedToSubscribers(Guid roomId, Guid sensorId)
        {
            lock (_roomsLock)
            {
                var sensor = GetHeatSensor(roomId, sensorId);
                if (sensor == null) return;
                SendToClientsResponse(_roomTemperatureSubscribers[roomId], 
                    XmlSubscribeResponseFactory.CreateRoomTemperatureSubscribeResponse(roomId, sensorId, sensor.Temperature));
            }
        }

        private void SendHeatSensorRemovedBroadcast(Guid roomId, Guid sensorId)
        {
            lock (_roomsLock)
            {
                SendBroadcastResponse(XmlBroadcastResponseFactory.CreateRemoveHeatSensorBroadcastResponse(roomId, sensorId));
            }
        }

        public override void Dispose()
        {
            ClearRooms();
            GC.SuppressFinalize(this);
        }

        private void HandleClientRequest(object? source, Guid clientId, Request request)
        {
            // get
            if (request.ContentType == RequestType.Get)
            {
                var getRequest = (GetRequestContent)request.Item;
                // all
                if (getRequest.DataType == GetRequestType.All)
                {
                    Console.WriteLine($"-> Get All Data Request from client: [{clientId}]");
                    
                    List<RoomDataContract> roomsDto = [];
                    lock (_roomsLock)
                    {
                        foreach (var room in _rooms)
                        {
                            List<HeaterDataContract> heaters = [];
                            foreach (var heater in room.Heaters)
                            {
                                heaters.Add(XmlDtoFactory.CreateHeaterDto(
                                    heater.Id,
                                    heater.Position.X,
                                    heater.Position.Y,
                                    heater.Temperature,
                                    heater.IsOn)
                                );
                            }

                            List<HeatSensorDataContract> sensors = [];
                            foreach (var sensor in room.HeatSensors)
                            {
                                sensors.Add(XmlDtoFactory.CreateHeatSensorDto(
                                    sensor.Id,
                                    sensor.Position.X,
                                    sensor.Position.Y,
                                    sensor.Temperature)
                                );
                            }

                            roomsDto.Add(XmlDtoFactory.CreateRoomDto(
                                room.Id,
                                room.Name,
                                room.Width,
                                room.Height,
                                heaters,
                                sensors)
                            );
                        }
                    }

                    SendClientResponse(clientId, XmlClientResponseFactory.CreateGetAllClientResponse(roomsDto));

                    Console.WriteLine($"-> All Data send to client: [{clientId}]");
                }
                // room
                else if (getRequest.DataType == GetRequestType.Room)
                {
                    var getRoomRequest = (GetRoomRequestData)getRequest.Item!;

                    Console.WriteLine($"-> Get Room [{getRoomRequest.RoomId}] Data Request from client: [{clientId}]");

                    var room = GetRoom(getRoomRequest.RoomId);
                    if (room == null)
                    {
                        Console.WriteLine($"-> Room [{getRoomRequest.RoomId} not found");
                        SendClientResponse(clientId, XmlClientResponseFactory.CreateGetRoomFailedClientResponse(getRoomRequest.RoomId));
                        Console.WriteLine($"-> Room [{getRoomRequest.RoomId}] Not Found sent to client: [{clientId}]");
                        return;
                    }
                    
                    Console.WriteLine($"-> Room {getRoomRequest.RoomId} founded");

                    List<HeaterDataContract> heatersDto = [];
                    foreach (var heater in room.Heaters)
                    {
                        var data = new HeaterDataContract
                        {
                            HeaterId = heater.Id,
                            IsOn = heater.IsOn,
                            Temperature = heater.Temperature,
                            X = heater.Position.X,
                            Y = heater.Position.Y
                        };
                        heatersDto.Add(data);
                    }

                    List<HeatSensorDataContract> heatSensorsDto = [];
                    foreach (var sensor in room.HeatSensors)
                    {
                        var data = new HeatSensorDataContract
                        {
                            HeatSensorId = sensor.Id,
                            Temperature = sensor.Temperature,
                            X = sensor.Position.X,
                            Y = sensor.Position.Y
                        };
                        heatSensorsDto.Add(data);
                    }

                    SendClientResponse(clientId, 
                        XmlClientResponseFactory.CreateGetRoomSuccessClientResponse(
                            room.Id, room.Name, room.Height, room.Width, heatersDto, heatSensorsDto
                            )
                        );

                    Console.WriteLine($"-> Room [{getRoomRequest.RoomId}] data send to client: [{clientId}]");
                }
                // heater
                else if (getRequest.DataType == GetRequestType.Heater)
                {
                    var getHeaterRequest = (GetHeaterRequestData)getRequest.Item!;

                    Console.WriteLine($"-> Get Heater [{getHeaterRequest.HeaterId}] Data from Room [{getHeaterRequest.RoomId}] " +
                                      $"Request from client: [{clientId}]");

                    var heater = GetHeater(getHeaterRequest.RoomId, getHeaterRequest.HeaterId);
                    if (heater == null)
                    {
                        Console.WriteLine($"-> Heater [{getHeaterRequest.HeaterId}] in Room [{getHeaterRequest.RoomId}] not found");
                        SendClientResponse(clientId,
                            XmlClientResponseFactory.CreateGetHeaterFailedClientResponse(getHeaterRequest.RoomId, getHeaterRequest.HeaterId));
                        Console.WriteLine($"-> Heater [{getHeaterRequest.HeaterId}] in Room [{getHeaterRequest.RoomId}] " +
                                          $"Not Found sent to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heater [{getHeaterRequest.HeaterId}] in Room [{getHeaterRequest.RoomId}] founded");

                    SendClientResponse(clientId,
                        XmlClientResponseFactory.CreateGetHeaterSuccessClientResponse(getHeaterRequest.RoomId, heater.Id,
                            heater.Position.X, heater.Position.Y, heater.Temperature, heater.IsOn));

                    Console.WriteLine($"-> Heater [{getHeaterRequest.HeaterId}] data from Room [{getHeaterRequest.RoomId}] " +
                                      $"send to client: [{clientId}]");
                }
                // heat sensor
                else if (getRequest.DataType == GetRequestType.HeatSensor)
                {
                    var getSensorRequest = (GetHeatSensorRequestData)getRequest.Item!;

                    Console.WriteLine($"-> Get Heat Sensor [{getSensorRequest.HeatSensorId}] Data from Room [{getSensorRequest.RoomId}] " +
                                      $"Request from client: [{clientId}]");

                    var sensor = GetHeatSensor(getSensorRequest.RoomId, getSensorRequest.HeatSensorId);
                    if (sensor == null)
                    {
                        Console.WriteLine(
                            $"-> Heat Sensor [{getSensorRequest.HeatSensorId}] in Room [{getSensorRequest.RoomId}] not found");
                        SendClientResponse(clientId,
                            XmlClientResponseFactory.CreateGetHeatSensorFailedClientResponse(getSensorRequest.RoomId,
                                getSensorRequest.HeatSensorId));
                        Console.WriteLine($"-> Heat Sensor [{getSensorRequest.HeatSensorId}] in Room [{getSensorRequest.RoomId}] " +
                                          $"Not Found sent to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heat Sensor [{getSensorRequest.HeatSensorId}] in Room [{getSensorRequest.RoomId}] founded");

                    SendClientResponse(clientId,
                        XmlClientResponseFactory.CreateGetHeatSensorSuccessClientResponse(getSensorRequest.RoomId, sensor.Id,
                            sensor.Position.X, sensor.Position.Y, sensor.Temperature));

                    Console.WriteLine($"-> Heat Sensor [{getSensorRequest.HeatSensorId}] Data from Room [{getSensorRequest.RoomId}] " +
                                      $"send to client: [{clientId}]");
                }
            }
            // add
            else if (request.ContentType == RequestType.Add)
            {
                var addRequest = (AddRequestContent)request.Item;
                // room
                if (addRequest.DataType == AddRequestType.Room)
                {
                    var addRoomRequest = (AddRoomRequestData)addRequest.Item;

                    Console.WriteLine($"-> Add Room \"{addRoomRequest.Name}\" {addRoomRequest.Width}m X {addRoomRequest.Height}m " +
                                      $"Request from client: [{clientId}]");

                    var roomId = AddRoom(addRoomRequest.Name, addRoomRequest.Width, addRoomRequest.Height);
                    if (roomId == Guid.Empty)
                    {
                        Console.WriteLine($"-> Room \"{addRoomRequest.Name}\" not added");
                        SendClientResponse(clientId, XmlClientResponseFactory.CreateAddRoomFailedClientResponse());
                        Console.WriteLine($"-> Room \"{addRoomRequest.Name}\" not added send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Room added with id [{roomId}]");

                    SendClientResponse(clientId, XmlClientResponseFactory.CreateAddRoomSuccessClientResponse(roomId));

                    Console.WriteLine($"-> Add Room [{roomId}] Result send to client: [{clientId}]");

                    SendRoomAddedBroadcast(roomId);

                    Console.WriteLine($"-> Added Room [{roomId}] Broadcasted to clients");
                }
                // heater
                else if (addRequest.DataType == AddRequestType.Heater)
                {
                    var addHeaterRequest = (AddHeaterRequestData)addRequest.Item;

                    Console.WriteLine($"-> Add Heater ({addHeaterRequest.X}, {addHeaterRequest.Y}) {addHeaterRequest.Temperature}\u00b0C " +
                                      $"to Room [{addHeaterRequest.RoomId}] Request from client: [{clientId}]");

                    var heaterId = AddHeater(addHeaterRequest.RoomId, addHeaterRequest.X, addHeaterRequest.Y,
                        addHeaterRequest.Temperature);
                    if (heaterId == Guid.Empty)
                    {
                        Console.WriteLine($"-> Heater not added to Room [{addHeaterRequest.RoomId}]");
                        SendClientResponse(clientId, XmlClientResponseFactory.CreateAddHeaterFailedClientResponse());
                        Console.WriteLine($"-> Heater not added to Room [{addHeaterRequest.RoomId}] send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heater added to Room [{addHeaterRequest.RoomId}] with id [{heaterId}]");

                    SendClientResponse(clientId, XmlClientResponseFactory.CreateAddHeaterSuccessClientResponse(addHeaterRequest.RoomId, heaterId));

                    Console.WriteLine($"-> Add Heater [{heaterId}] to Room [{addHeaterRequest.RoomId}] result send to client: [{clientId}]");

                    SendHeaterAddedBroadcast(addHeaterRequest.RoomId, heaterId);

                    Console.WriteLine($"-> Added Heater [{heaterId}] to Room [{addHeaterRequest.RoomId}] Broadcasted to clients");
                }
                // heat sensor
                else if (addRequest.DataType == AddRequestType.HeatSensor)
                {
                    var addSensorRequest = (AddHeatSensorRequestData)addRequest.Item;

                    Console.WriteLine($"-> Add Heat Sensor ({addSensorRequest.X}, {addSensorRequest.Y}) to Room [{addSensorRequest.RoomId}] " +
                                      $"Request from client: [{clientId}]");

                    var sensorId = AddHeatSensor(addSensorRequest.RoomId, addSensorRequest.X, addSensorRequest.Y);
                    if (sensorId == Guid.Empty)
                    {
                        Console.WriteLine($"-> Heat Sensor not added to Room [{addSensorRequest.RoomId}]");
                        SendClientResponse(clientId, XmlClientResponseFactory.CreateAddHeatSensorFailedClientResponse());
                        Console.WriteLine($"-> Heat Sensor not added to Room [{addSensorRequest.RoomId}] send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heat Sensor added to Room [{addSensorRequest.RoomId}] with id [{sensorId}]");

                    SendClientResponse(clientId, XmlClientResponseFactory.CreateAddHeatSensorSuccessClientResponse(addSensorRequest.RoomId, sensorId));

                    Console.WriteLine($"-> Add Heat Sensor [{sensorId}] to Room [{addSensorRequest.RoomId}] result send to client: [{clientId}]");

                    SendHeatSensorAddedBroadcast(addSensorRequest.RoomId, sensorId);

                    Console.WriteLine($"-> Added Heat Sensor [{sensorId}] to Room [{addSensorRequest.RoomId}] Broadcasted to clients");
                }
            }
            // update
            else if (request.ContentType == RequestType.Update)
            {
                var updateRequest = (UpdateRequestContent)request.Item;
                // heater
                if (updateRequest.DataType == UpdateRequestType.Heater)
                {
                    var updateHeaterRequest = (UpdateHeaterRequestData)updateRequest.Item;

                    Console.WriteLine($"-> Update Heater {updateHeaterRequest.HeaterId} ({updateHeaterRequest.X}, {updateHeaterRequest.Y})" +
                                      $" {updateHeaterRequest.Temperature}\u00b0C On:{updateHeaterRequest.IsOn} in Room " +
                                      $"{updateHeaterRequest.RoomId} Request from client: {clientId}");

                    if (!UpdateHeater(updateHeaterRequest.RoomId, updateHeaterRequest.HeaterId,
                            updateHeaterRequest.X, updateHeaterRequest.Y, updateHeaterRequest.Temperature,
                            updateHeaterRequest.IsOn))
                    {
                        Console.WriteLine($"-> Heater [{updateHeaterRequest.HeaterId}] in Room [{updateHeaterRequest.RoomId}] not updated");
                        SendClientResponse(clientId, XmlClientResponseFactory.CreateUpdateHeaterClientResponse(
                            updateHeaterRequest.RoomId, updateHeaterRequest.HeaterId, false));
                        Console.WriteLine(
                            $"-> Heater [{updateHeaterRequest.HeaterId}] in Room [{updateHeaterRequest.RoomId}]" +
                            $" not updated send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heater [{updateHeaterRequest.HeaterId}] in Room [{updateHeaterRequest.RoomId}] updated");

                    SendClientResponse(clientId, XmlClientResponseFactory.CreateUpdateHeaterClientResponse(
                        updateHeaterRequest.RoomId, updateHeaterRequest.HeaterId, true));

                    Console.WriteLine($"-> Update Heater [{updateHeaterRequest.HeaterId}] in Room [{updateHeaterRequest.RoomId}] result " +
                                      $"send to client: [{clientId}]");

                    SendHeaterUpdatedBroadcast(updateHeaterRequest.RoomId, updateHeaterRequest.HeaterId);

                    Console.WriteLine($"-> Updated Heater [{updateHeaterRequest.HeaterId}] from Room [{updateHeaterRequest.RoomId}] " +
                                      $"Broadcasted to clients");
                }
                // heat sensor
                else if (updateRequest.DataType == UpdateRequestType.HeatSensor)
                {
                    var updateSensorRequest = (UpdateHeatSensorRequestData)updateRequest.Item;

                    Console.WriteLine($"-> Update Heat Sensor [{updateSensorRequest.HeatSensorId}]" +
                                      $" ({updateSensorRequest.X}, {updateSensorRequest.Y})" +
                                      $" in Room {updateSensorRequest.RoomId} Request from client: [{clientId}]");

                    if (!UpdateHeatSensor(updateSensorRequest.RoomId, updateSensorRequest.HeatSensorId,
                            updateSensorRequest.X, updateSensorRequest.Y))
                    {
                        Console.WriteLine($"-> Heat Sensor [{updateSensorRequest.HeatSensorId}] in Room [{updateSensorRequest.RoomId}] " +
                                          $"not updated");
                        SendClientResponse(clientId, XmlClientResponseFactory.CreateUpdateHeatSensorClientResponse(
                            updateSensorRequest.RoomId, updateSensorRequest.HeatSensorId, false));
                        Console.WriteLine($"-> Heat Sensor [{updateSensorRequest.HeatSensorId}] in Room [{updateSensorRequest.RoomId}] " +
                                          $"not updated send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heat Sensor [{updateSensorRequest.HeatSensorId}] in Room [{updateSensorRequest.RoomId}] updated");

                    SendClientResponse(clientId, XmlClientResponseFactory.CreateUpdateHeatSensorClientResponse(
                        updateSensorRequest.RoomId, updateSensorRequest.HeatSensorId, true));

                    Console.WriteLine($"-> Update Heat Sensor [{updateSensorRequest.HeatSensorId}] in Room [{updateSensorRequest.RoomId}] result " +
                                      $"send to client: [{clientId}]");

                    SendHeatSensorUpdatedBroadcast(updateSensorRequest.RoomId, updateSensorRequest.HeatSensorId);

                    Console.WriteLine($"-> Updated Heat Sensor [{updateSensorRequest.HeatSensorId}] from Room [{updateSensorRequest.RoomId}] " +
                                      $"Broadcast to other clients");
                }
            }
            // remove
            else if (request.ContentType == RequestType.Remove)
            {
                var removeRequest = (RemoveRequestContent)request.Item;
                // room
                if (removeRequest.DataType == RemoveRequestType.Room)
                {
                    var removeRoomRequest = (RemoveRoomRequestData)removeRequest.Item;

                    Console.WriteLine($"-> Remove Room [{removeRoomRequest.RoomId}] Request from client: [{clientId}]");

                    if (!RemoveRoom(removeRoomRequest.RoomId))
                    {
                        Console.WriteLine($"-> Room [{removeRoomRequest.RoomId}] not removed");
                        SendClientResponse(clientId, XmlClientResponseFactory.CreateRemoveRoomClientResponse(removeRoomRequest.RoomId, false));
                        Console.WriteLine($"-> Room [{removeRoomRequest.RoomId}] not removed send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Room [{removeRoomRequest.RoomId}] removed");

                    SendClientResponse(clientId, XmlClientResponseFactory.CreateRemoveRoomClientResponse(removeRoomRequest.RoomId, true));

                    Console.WriteLine($"-> Remove Room [{removeRoomRequest.RoomId}] result send to client: [{clientId}]");

                    SendRoomRemovedBroadcast(removeRoomRequest.RoomId);

                    Console.WriteLine($"-> Removed Room [{removeRoomRequest.RoomId}] Broadcasted to clients");
                }
                // heater
                else if (removeRequest.DataType == RemoveRequestType.Heater)
                {
                    var removeHeaterRequest = (RemoveHeaterRequestData)removeRequest.Item;

                    Console.WriteLine($"-> Remove Heater [{removeHeaterRequest.HeaterId}] from Room [{removeHeaterRequest.RoomId}] " +
                                      $"Request from client: [{clientId}]");

                    if (!RemoveHeater(removeHeaterRequest.RoomId, removeHeaterRequest.HeaterId))
                    {
                        Console.WriteLine($"-> Heater [{removeHeaterRequest.HeaterId}] from Room [{removeHeaterRequest.RoomId}] not removed");
                        SendClientResponse(clientId, XmlClientResponseFactory.CreateRemoveHeaterClientResponse(
                            removeHeaterRequest.RoomId, removeHeaterRequest.HeaterId, false));
                        Console.WriteLine($"-> Heater [{removeHeaterRequest.HeaterId}] from Room [{removeHeaterRequest.RoomId}] not removed " +
                                          $"send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heater [{removeHeaterRequest.HeaterId}] from Room [{removeHeaterRequest.RoomId}] removed");

                    SendClientResponse(clientId, XmlClientResponseFactory.CreateRemoveHeaterClientResponse(
                        removeHeaterRequest.RoomId, removeHeaterRequest.HeaterId, true));

                    Console.WriteLine($"-> Remove Heater [{removeHeaterRequest.HeaterId}] from Room [{removeHeaterRequest.RoomId}] result " +
                                                        $"send to client: [{clientId}]");

                    SendHeaterRemovedBroadcast(removeHeaterRequest.RoomId, removeHeaterRequest.HeaterId);

                    Console.WriteLine($"-> Removed Heater [{removeHeaterRequest.HeaterId}] from Room [{removeHeaterRequest.RoomId}] " +
                                                        $"Broadcast to other clients");
                }
                // heat sensor
                else if (removeRequest.DataType == RemoveRequestType.HeatSensor)
                {
                    var removeSensorRequest = (RemoveHeatSensorRequestData)removeRequest.Item;

                    Console.WriteLine($"-> Remove Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room [{removeSensorRequest.RoomId}] " +
                                      $"Request from client: [{clientId}]");

                    if (!RemoveHeatSensor(removeSensorRequest.RoomId, removeSensorRequest.HeatSensorId))
                    {
                        Console.WriteLine($"-> Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room " +
                                          $"[{removeSensorRequest.RoomId}] not removed");

                        SendClientResponse(clientId, XmlClientResponseFactory.CreateRemoveHeatSensorClientResponse(
                            removeSensorRequest.RoomId, removeSensorRequest.HeatSensorId, false));

                        Console.WriteLine($"-> Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room [{removeSensorRequest.RoomId}] " +
                                          $"not removed send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room [{removeSensorRequest.RoomId}] removed");

                    SendClientResponse(clientId, XmlClientResponseFactory.CreateRemoveHeatSensorClientResponse(
                        removeSensorRequest.RoomId, removeSensorRequest.HeatSensorId, true));

                    Console.WriteLine($"-> Remove Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room [{removeSensorRequest.RoomId}] result" +
                                      $" send to client: [{clientId}]");

                    SendHeatSensorRemovedBroadcast(removeSensorRequest.RoomId, removeSensorRequest.HeatSensorId);

                    Console.WriteLine($"-> Removed Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room [{removeSensorRequest.RoomId}] " +
                                      $"Broadcasted to clients");
                }
            }
            // subscribe
            else if (request.ContentType == RequestType.Subscribe)
            {
                var subscribeRequest = (SubscribeRequestContent)request.Item;
                // room temperature
                if (subscribeRequest.DataType == SubscribeRequestType.RoomTemperature)
                {
                    var roomTemperatureSubscribeRequest = (SubscribeRoomTemperatureRequestData)subscribeRequest.RoomTemperatureData;

                    Console.WriteLine($"-> Subscribe To Room [{roomTemperatureSubscribeRequest.RoomId}] " +
                                      $"Temperature Request from client: [{clientId}]");

                    if (_rooms.All(room => room.Id != roomTemperatureSubscribeRequest.RoomId))
                    {
                        Console.WriteLine($"-> Failed to subscribe to Room [{roomTemperatureSubscribeRequest.RoomId}] Temperature " +
                                          $"because room doesn't exist");

                        SendClientResponse(clientId, XmlClientResponseFactory.CreateSubscribeRoomTemperatureClientResponse(
                            roomTemperatureSubscribeRequest.RoomId, false));

                        Console.WriteLine($"-> Subscribe To Room [{roomTemperatureSubscribeRequest.RoomId}] Temperature " +
                                          $"result send to client: [{clientId}]");
                        return;
                    }

                    if (!_roomTemperatureSubscribers.ContainsKey(roomTemperatureSubscribeRequest.RoomId))
                    {
                        _roomTemperatureSubscribers[roomTemperatureSubscribeRequest.RoomId] = [];
                    }

                    if (_roomTemperatureSubscribers[roomTemperatureSubscribeRequest.RoomId].Contains(clientId))
                    {
                        Console.WriteLine($"-> Failed to subscribe to Room [{roomTemperatureSubscribeRequest.RoomId}] Temperature " +
                                          $"because client already subscribed");

                        SendClientResponse(clientId, XmlClientResponseFactory.CreateSubscribeRoomTemperatureClientResponse(
                            roomTemperatureSubscribeRequest.RoomId, false));

                        Console.WriteLine($"-> Subscribe To Room [{roomTemperatureSubscribeRequest.RoomId}] Temperature " +
                                          $"result send to client: [{clientId}]");
                        return;
                    }

                    _roomTemperatureSubscribers[roomTemperatureSubscribeRequest.RoomId].Add(clientId);

                    Console.WriteLine($"-> Successfully subscribed to Room [{roomTemperatureSubscribeRequest.RoomId}] Temperature");

                    SendClientResponse(clientId, XmlClientResponseFactory.CreateSubscribeRoomTemperatureClientResponse(
                        roomTemperatureSubscribeRequest.RoomId, true));

                    Console.WriteLine($"-> Subscribe To Room [{roomTemperatureSubscribeRequest.RoomId}] Temperature " +
                                      $"result send to client: [{clientId}]");
                }
            }
            // unsubscribe
            else if (request.ContentType == RequestType.Unsubscribe)
            {
                var unsubscribeRequest = (UnsubscribeRequestContent)request.Item;
                // room temperature
                if (unsubscribeRequest.DataType == UnsubscribeRequestType.RoomTemperature)
                {
                    var roomTemperatureUnsubscribeRequest = (UnsubscribeRoomTemperatureRequestData)unsubscribeRequest.RoomTemperatureData;

                    Console.WriteLine($"-> Unsubscribe From Room [{roomTemperatureUnsubscribeRequest.RoomId}] " +
                                      $"Temperature Request from client: [{clientId}]");

                    if (_rooms.All(room => room.Id != roomTemperatureUnsubscribeRequest.RoomId))
                    {
                        Console.WriteLine($"-> Failed to unsubscribe From Room [{roomTemperatureUnsubscribeRequest.RoomId}] Temperature " +
                                          $"because room doesn't exist");

                        SendClientResponse(clientId, XmlClientResponseFactory.CreateUnsubscribeRoomTemperatureClientResponse(
                            roomTemperatureUnsubscribeRequest.RoomId, false));

                        Console.WriteLine($"-> Unsubscribe From Room [{roomTemperatureUnsubscribeRequest.RoomId}] Temperature " +
                                          $"result send to client: [{clientId}]");
                        return;
                    }

                    if (!_roomTemperatureSubscribers.ContainsKey(roomTemperatureUnsubscribeRequest.RoomId))
                    {
                        Console.WriteLine($"-> Failed to unsubscribe From Room [{roomTemperatureUnsubscribeRequest.RoomId}] Temperature " +
                                          $"because client wasn't subscribed");

                        SendClientResponse(clientId, XmlClientResponseFactory.CreateUnsubscribeRoomTemperatureClientResponse(
                            roomTemperatureUnsubscribeRequest.RoomId, false));

                        Console.WriteLine($"-> Unsubscribe From Room [{roomTemperatureUnsubscribeRequest.RoomId}] Temperature " +
                                          $"result send to client: [{clientId}]");
                        return;
                    }

                    if (!_roomTemperatureSubscribers[roomTemperatureUnsubscribeRequest.RoomId].Contains(clientId))
                    {
                        Console.WriteLine($"-> Failed to unsubscribe From Room [{roomTemperatureUnsubscribeRequest.RoomId}] Temperature " +
                                          $"because client wasn't subscribed");

                        SendClientResponse(clientId, XmlClientResponseFactory.CreateUnsubscribeRoomTemperatureClientResponse(
                            roomTemperatureUnsubscribeRequest.RoomId, false));

                        Console.WriteLine($"-> Unsubscribe From Room [{roomTemperatureUnsubscribeRequest.RoomId}] Temperature " +
                                          $"result send to client: [{clientId}]");
                        return;
                    }

                    _roomTemperatureSubscribers[roomTemperatureUnsubscribeRequest.RoomId].Remove(clientId);

                    Console.WriteLine($"-> Successfully unsubscribed From Room [{roomTemperatureUnsubscribeRequest.RoomId}] Temperature");

                    SendClientResponse(clientId, XmlClientResponseFactory.CreateUnsubscribeRoomTemperatureClientResponse(
                        roomTemperatureUnsubscribeRequest.RoomId, true));

                    Console.WriteLine($"-> Unsubscribe From Room [{roomTemperatureUnsubscribeRequest.RoomId}] Temperature " +
                                      $"result send to client: [{clientId}]");
                }
            }
        }
    }
}
