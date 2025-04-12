using TPUM.Server.Logic;

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
                    Console.WriteLine($"-> Position Changed in Heater [{heater.Id}]. Change Broadcasted to clients");
                    SendHeaterUpdatedBroadcast(roomId, heater.Id);
                    break;
                case IHeatSensor sensor:
                    Console.WriteLine($"-> Position Changed in Heat Sensor [{sensor.Id}]. Change Broadcasted to clients");
                    SendHeatSensorUpdatedBroadcast(roomId, sensor.Id);
                    break;
            }
        }

        private void GetTemperatureChange(Guid roomId, object? source, float lastTemperature, float newTemperature)
        {
            switch (source)
            {
                case IHeater heater:
                    Console.WriteLine($"-> Temperature Changed in Heater [{heater.Id}]. Change Broadcasted to clients");
                    SendHeaterUpdatedBroadcast(roomId, heater.Id);
                    break;
                case IHeatSensor sensor:
                    Console.WriteLine($"-> Temperature Changed in Heat Sensor [{sensor.Id}]. Change Broadcasted to clients");
                    SendHeatSensorUpdatedBroadcast(roomId, sensor.Id);
                    break;
            }
        }

        private void GetEnableChange(Guid roomId, object? source, bool lastEnable, bool newEnable)
        {
            switch (source)
            {
                case IHeater heater:
                    Console.WriteLine($"-> Enable Changed in Heater [{heater.Id}]. Change Broadcasted to clients");
                    SendHeaterUpdatedBroadcast(roomId, heater.Id);
                    break;
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
                SendBroadcastResponse(XmlResponseFactory.CreateAddRoomBroadcastResponse(roomId, room.Name, room.Width, room.Height));
            }
        }

        private void SendRoomRemovedBroadcast(Guid roomId)
        {
            lock (_roomsLock)
            {
                SendBroadcastResponse(XmlResponseFactory.CreateRemoveRoomBroadcastResponse(roomId));
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
                SendBroadcastResponse(XmlResponseFactory.CreateAddHeaterBroadcastResponse(roomId, heaterId, heater.Position.X, 
                    heater.Position.Y, heater.Temperature));
            }
        }

        private void SendHeaterUpdatedBroadcast(Guid roomId, Guid heaterId)
        {
            lock (_roomsLock)
            {
                var heater = GetHeater(roomId, heaterId);
                if (heater == null) return;
                SendBroadcastResponse(XmlResponseFactory.CreateUpdateHeaterBroadcastResponse(roomId, heaterId, 
                    heater.Position.X, heater.Position.Y, heater.Temperature, heater.IsOn));
            }
        }

        private void SendHeaterRemovedBroadcast(Guid roomId, Guid heaterId)
        {
            lock (_roomsLock)
            {
                SendBroadcastResponse(XmlResponseFactory.CreateRemoveHeaterBroadcastResponse(roomId, heaterId));
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
                SendBroadcastResponse(XmlResponseFactory.CreateAddHeatSensorBroadcastResponse(roomId, sensorId, 
                    sensor.Position.X, sensor.Position.Y, sensor.Temperature));
            }
        }

        private void SendHeatSensorUpdatedBroadcast(Guid roomId, Guid sensorId)
        {
            lock (_roomsLock)
            {
                var sensor = GetHeatSensor(roomId, sensorId);
                if (sensor == null) return;
                SendBroadcastResponse(XmlResponseFactory.CreateUpdateHeatSensorBroadcastResponse(roomId, sensorId, 
                    sensor.Position.X, sensor.Position.Y, sensor.Temperature));
            }
        }

        private void SendHeatSensorRemovedBroadcast(Guid roomId, Guid sensorId)
        {
            lock (_roomsLock)
            {
                SendBroadcastResponse(XmlResponseFactory.CreateRemoveHeatSensorBroadcastResponse(roomId, sensorId));
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
                var getRequest = (GetRequestContent)request.Content;
                // all
                if (getRequest.DataType == GetRequestType.All)
                {
                    Console.WriteLine($"-> Get All Data Request from client: [{clientId}]");
                    
                    List<RoomDataContract> roomsDto = [];
                    lock (_roomsLock)
                    {
                        foreach (var room in _rooms)
                        {
                            var roomData = new RoomDataContract
                            {
                                RoomId = room.Id,
                                Name = room.Name,
                                Height = room.Height,
                                Width = room.Width
                            };

                            foreach (var heater in room.Heaters)
                            {
                                var heaterData = new HeaterDataContract
                                {
                                    HeaterId = heater.Id,
                                    IsOn = heater.IsOn,
                                    Temperature = heater.Temperature,
                                    X = heater.Position.X,
                                    Y = heater.Position.Y
                                };
                                roomData.Heaters.Add(heaterData);
                            }

                            foreach (var sensor in room.HeatSensors)
                            {
                                var sensorData = new HeatSensorDataContract
                                {
                                    HeatSensorId = sensor.Id,
                                    Temperature = sensor.Temperature,
                                    X = sensor.Position.X,
                                    Y = sensor.Position.Y
                                };
                                roomData.HeatSensors.Add(sensorData);
                            }

                            roomsDto.Add(roomData);
                        }
                    }

                    SendClientResponse(clientId, XmlResponseFactory.CreateGetAllClientResponse(roomsDto));

                    Console.WriteLine($"-> All Data send to client: [{clientId}]");
                }
                // room
                else if (getRequest.DataType == GetRequestType.Room)
                {
                    var getRoomRequest = (GetRoomRequestData)getRequest.Data!;

                    Console.WriteLine($"-> Get Room [{getRoomRequest.RoomId}] Data Request from client: [{clientId}]");

                    var room = GetRoom(getRoomRequest.RoomId);
                    if (room == null)
                    {
                        Console.WriteLine($"-> Room [{getRoomRequest.RoomId} not found");
                        SendClientResponse(clientId, XmlResponseFactory.CreateGetRoomFailedClientResponse(getRoomRequest.RoomId));
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
                        XmlResponseFactory.CreateGetRoomSuccessClientResponse(
                            room.Id, room.Name, room.Height, room.Width, heatersDto, heatSensorsDto
                            )
                        );

                    Console.WriteLine($"-> Room [{getRoomRequest.RoomId}] data send to client: [{clientId}]");
                }
                // heater
                else if (getRequest.DataType == GetRequestType.Heater)
                {
                    var getHeaterRequest = (GetHeaterRequestData)getRequest.Data!;

                    Console.WriteLine($"-> Get Heater [{getHeaterRequest.HeaterId}] Data from Room [{getHeaterRequest.RoomId}] " +
                                      $"Request from client: [{clientId}]");

                    var heater = GetHeater(getHeaterRequest.RoomId, getHeaterRequest.HeaterId);
                    if (heater == null)
                    {
                        Console.WriteLine($"-> Heater [{getHeaterRequest.HeaterId}] in Room [{getHeaterRequest.RoomId}] not found");
                        SendClientResponse(clientId,
                            XmlResponseFactory.CreateGetHeaterFailedClientResponse(getHeaterRequest.RoomId, getHeaterRequest.HeaterId));
                        Console.WriteLine($"-> Heater [{getHeaterRequest.HeaterId}] in Room [{getHeaterRequest.RoomId}] " +
                                          $"Not Found sent to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heater [{getHeaterRequest.HeaterId}] in Room [{getHeaterRequest.RoomId}] founded");

                    SendClientResponse(clientId,
                        XmlResponseFactory.CreateGetHeaterSuccessClientResponse(getHeaterRequest.RoomId, heater.Id,
                            heater.Position.X, heater.Position.Y, heater.Temperature, heater.IsOn));

                    Console.WriteLine($"-> Heater [{getHeaterRequest.HeaterId}] data from Room [{getHeaterRequest.RoomId}] " +
                                      $"send to client: [{clientId}]");
                }
                // heat sensor
                else if (getRequest.DataType == GetRequestType.HeatSensor)
                {
                    var getSensorRequest = (GetHeatSensorRequestData)getRequest.Data!;

                    Console.WriteLine($"-> Get Heat Sensor [{getSensorRequest.HeatSensorId}] Data from Room [{getSensorRequest.RoomId}] " +
                                      $"Request from client: [{clientId}]");

                    var sensor = GetHeatSensor(getSensorRequest.RoomId, getSensorRequest.HeatSensorId);
                    if (sensor == null)
                    {
                        Console.WriteLine(
                            $"-> Heat Sensor [{getSensorRequest.HeatSensorId}] in Room [{getSensorRequest.RoomId}] not found");
                        SendClientResponse(clientId,
                            XmlResponseFactory.CreateGetHeatSensorFailedClientResponse(getSensorRequest.RoomId,
                                getSensorRequest.HeatSensorId));
                        Console.WriteLine($"-> Heat Sensor [{getSensorRequest.HeatSensorId}] in Room [{getSensorRequest.RoomId}] " +
                                          $"Not Found sent to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heat Sensor [{getSensorRequest.HeatSensorId}] in Room [{getSensorRequest.RoomId}] founded");

                    SendClientResponse(clientId,
                        XmlResponseFactory.CreateGetHeatSensorSuccessClientResponse(getSensorRequest.RoomId, sensor.Id,
                            sensor.Position.X, sensor.Position.Y, sensor.Temperature));

                    Console.WriteLine($"-> Heat Sensor [{getSensorRequest.HeatSensorId}] Data from Room [{getSensorRequest.RoomId}] " +
                                      $"send to client: [{clientId}]");
                }
            }
            // add
            else if (request.ContentType == RequestType.Add)
            {
                var addRequest = (AddRequestContent)request.Content;
                // room
                if (addRequest.DataType == AddRequestType.Room)
                {
                    var addRoomRequest = (AddRoomRequestData)addRequest.Data;

                    Console.WriteLine($"-> Add Room \"{addRoomRequest.Name}\" {addRoomRequest.Width} mX{addRoomRequest.Height} m " +
                                      $"Request from client: [{clientId}]");

                    var roomId = AddRoom(addRoomRequest.Name, addRoomRequest.Width, addRoomRequest.Height);
                    if (roomId == Guid.Empty)
                    {
                        Console.WriteLine($"-> Room \"{addRoomRequest.Name}\" not added");
                        SendClientResponse(clientId, XmlResponseFactory.CreateAddRoomFailedClientResponse());
                        Console.WriteLine($"-> Room \"{addRoomRequest.Name}\" not added send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Room added with id [{roomId}]");

                    SendClientResponse(clientId, XmlResponseFactory.CreateAddRoomSuccessClientResponse(roomId));

                    Console.WriteLine($"-> Add Room [{roomId}] Result send to client: [{clientId}]");

                    SendRoomAddedBroadcast(roomId);

                    Console.WriteLine($"-> Added Room [{roomId}] Broadcasted to clients");
                }
                // heater
                else if (addRequest.DataType == AddRequestType.Heater)
                {
                    var addHeaterRequest = (AddHeaterRequestData)addRequest.Data;

                    Console.WriteLine($"-> Add Heater ({addHeaterRequest.X}, {addHeaterRequest.Y}) {addHeaterRequest.Temperature}\u00b0C " +
                                      $"to Room [{addHeaterRequest.RoomId}] Request from client: [{clientId}]");

                    var heaterId = AddHeater(addHeaterRequest.RoomId, addHeaterRequest.X, addHeaterRequest.Y,
                        addHeaterRequest.Temperature);
                    if (heaterId == Guid.Empty)
                    {
                        Console.WriteLine($"-> Heater not added to Room [{addHeaterRequest.RoomId}]");
                        SendClientResponse(clientId, XmlResponseFactory.CreateAddHeaterFailedClientResponse());
                        Console.WriteLine($"-> Heater not added to Room [{addHeaterRequest.RoomId}] send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heater added to Room [{addHeaterRequest.RoomId}] with id [{heaterId}]");

                    SendClientResponse(clientId, XmlResponseFactory.CreateAddHeaterSuccessClientResponse(addHeaterRequest.RoomId, heaterId));

                    Console.WriteLine($"-> Add Heater [{heaterId}] to Room [{addHeaterRequest.RoomId}] result send to client: [{clientId}]");

                    SendHeaterAddedBroadcast(addHeaterRequest.RoomId, heaterId);

                    Console.WriteLine($"-> Added Heater [{heaterId}] to Room [{addHeaterRequest.RoomId}] Broadcasted to clients\n");
                }
                // heat sensor
                else if (addRequest.DataType == AddRequestType.HeatSensor)
                {
                    var addSensorRequest = (AddHeatSensorRequestData)addRequest.Data;

                    Console.WriteLine($"-> Add Heat Sensor ({addSensorRequest.X}, {addSensorRequest.Y}) to Room [{addSensorRequest.RoomId}] " +
                                      $"Request from client: [{clientId}]");

                    var sensorId = AddHeatSensor(addSensorRequest.RoomId, addSensorRequest.X, addSensorRequest.Y);
                    if (sensorId == Guid.Empty)
                    {
                        Console.WriteLine($"-> Heat Sensor not added to Room [{addSensorRequest.RoomId}]");
                        SendClientResponse(clientId, XmlResponseFactory.CreateAddHeatSensorFailedClientResponse());
                        Console.WriteLine($"-> Heat Sensor not added to Room [{addSensorRequest.RoomId}] send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heat Sensor added to Room [{addSensorRequest.RoomId}] with id [{sensorId}]");

                    SendClientResponse(clientId, XmlResponseFactory.CreateAddHeatSensorSuccessClientResponse(addSensorRequest.RoomId, sensorId));

                    Console.WriteLine($"-> Add Heat Sensor [{sensorId}] to Room [{addSensorRequest.RoomId}] result send to client: [{clientId}]");

                    SendHeatSensorAddedBroadcast(addSensorRequest.RoomId, sensorId);

                    Console.WriteLine($"-> Added Heat Sensor [{sensorId}] to Room [{addSensorRequest.RoomId}] Broadcasted to clients");
                }
            }
            // update
            else if (request.ContentType == RequestType.Update)
            {
                var updateRequest = (UpdateRequestContent)request.Content;
                // heater
                if (updateRequest.DataType == UpdateRequestType.Heater)
                {
                    var updateHeaterRequest = (UpdateHeaterRequestData)updateRequest.Data;

                    Console.WriteLine($"-> Update Heater {updateHeaterRequest.HeaterId} ({updateHeaterRequest.X}, {updateHeaterRequest.Y})" +
                                      $" {updateHeaterRequest.Temperature}\u00b0C On:{updateHeaterRequest.IsOn} in Room " +
                                      $"{updateHeaterRequest.RoomId} Request from client: {clientId}");

                    if (!UpdateHeater(updateHeaterRequest.RoomId, updateHeaterRequest.HeaterId,
                            updateHeaterRequest.X, updateHeaterRequest.Y, updateHeaterRequest.Temperature,
                            updateHeaterRequest.IsOn))
                    {
                        Console.WriteLine($"-> Heater [{updateHeaterRequest.HeaterId}] in Room [{updateHeaterRequest.RoomId}] not updated");
                        SendClientResponse(clientId, XmlResponseFactory.CreateUpdateHeaterClientResponse(
                            updateHeaterRequest.RoomId, updateHeaterRequest.HeaterId, false));
                        Console.WriteLine(
                            $"-> Heater [{updateHeaterRequest.HeaterId}] in Room [{updateHeaterRequest.RoomId}]" +
                            $" not updated send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heater [{updateHeaterRequest.HeaterId}] in Room [{updateHeaterRequest.RoomId}] updated");

                    SendClientResponse(clientId, XmlResponseFactory.CreateUpdateHeaterClientResponse(
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
                    var updateSensorRequest = (UpdateHeatSensorRequestData)updateRequest.Data;

                    Console.WriteLine($"-> Update Heat Sensor [{updateSensorRequest.HeatSensorId}]" +
                                      $" ({updateSensorRequest.X}, {updateSensorRequest.Y})" +
                                      $" in Room {updateSensorRequest.RoomId} Request from client: [{clientId}]");

                    if (!UpdateHeatSensor(updateSensorRequest.RoomId, updateSensorRequest.HeatSensorId,
                            updateSensorRequest.X, updateSensorRequest.Y))
                    {
                        Console.WriteLine($"-> Heat Sensor [{updateSensorRequest.HeatSensorId}] in Room [{updateSensorRequest.RoomId}] " +
                                          $"not updated");
                        SendClientResponse(clientId, XmlResponseFactory.CreateUpdateHeatSensorClientResponse(
                            updateSensorRequest.RoomId, updateSensorRequest.HeatSensorId, false));
                        Console.WriteLine($"-> Heat Sensor [{updateSensorRequest.HeatSensorId}] in Room [{updateSensorRequest.RoomId}] " +
                                          $"not updated send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heat Sensor [{updateSensorRequest.HeatSensorId}] in Room [{updateSensorRequest.RoomId}] updated");

                    SendClientResponse(clientId, XmlResponseFactory.CreateUpdateHeatSensorClientResponse(
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
                var removeRequest = (RemoveRequestContent)request.Content;
                // room
                if (removeRequest.DataType == RemoveRequestType.Room)
                {
                    var removeRoomRequest = (RemoveRoomRequestData)removeRequest.Data;

                    Console.WriteLine($"-> Remove Room [{removeRoomRequest.RoomId}] Request from client: [{clientId}]");

                    if (!RemoveRoom(removeRoomRequest.RoomId))
                    {
                        Console.WriteLine($"-> Room [{removeRoomRequest.RoomId}] not removed");
                        SendClientResponse(clientId, XmlResponseFactory.CreateRemoveRoomClientResponse(removeRoomRequest.RoomId, false));
                        Console.WriteLine($"-> Room [{removeRoomRequest.RoomId}] not removed send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Room [{removeRoomRequest.RoomId}] removed");

                    SendClientResponse(clientId, XmlResponseFactory.CreateRemoveRoomClientResponse(removeRoomRequest.RoomId, true));

                    Console.WriteLine($"-> Remove Room [{removeRoomRequest.RoomId}] result send to client: [{clientId}]");

                    SendRoomRemovedBroadcast(removeRoomRequest.RoomId);

                    Console.WriteLine($"-> Removed Room [{removeRoomRequest.RoomId}] Broadcasted to clients");
                }
                // heater
                else if (removeRequest.DataType == RemoveRequestType.Heater)
                {
                    var removeHeaterRequest = (RemoveHeaterRequestData)removeRequest.Data;

                    Console.WriteLine($"-> Remove Heater [{removeHeaterRequest.HeaterId}] from Room [{removeHeaterRequest.RoomId}] " +
                                      $"Request from client: [{clientId}]");

                    if (!RemoveHeater(removeHeaterRequest.RoomId, removeHeaterRequest.HeaterId))
                    {
                        Console.WriteLine($"-> Heater [{removeHeaterRequest.HeaterId}] from Room [{removeHeaterRequest.RoomId}] not removed");
                        SendClientResponse(clientId, XmlResponseFactory.CreateRemoveHeaterClientResponse(
                            removeHeaterRequest.RoomId, removeHeaterRequest.HeaterId, false));
                        Console.WriteLine($"-> Heater [{removeHeaterRequest.HeaterId}] from Room [{removeHeaterRequest.RoomId}] not removed " +
                                          $"send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heater [{removeHeaterRequest.HeaterId}] from Room [{removeHeaterRequest.RoomId}] removed");

                    SendClientResponse(clientId, XmlResponseFactory.CreateRemoveHeaterClientResponse(
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
                    var removeSensorRequest = (RemoveHeatSensorRequestData)removeRequest.Data;

                    Console.WriteLine($"-> Remove Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room [{removeSensorRequest.RoomId}] " +
                                      $"Request from client: [{clientId}]");

                    if (!RemoveHeatSensor(removeSensorRequest.RoomId, removeSensorRequest.HeatSensorId))
                    {
                        Console.WriteLine($"-> Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room " +
                                          $"[{removeSensorRequest.RoomId}] not removed");

                        SendClientResponse(clientId, XmlResponseFactory.CreateRemoveHeatSensorClientResponse(
                            removeSensorRequest.RoomId, removeSensorRequest.HeatSensorId, false));

                        Console.WriteLine($"-> Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room [{removeSensorRequest.RoomId}] " +
                                          $"not removed send to client: [{clientId}]");
                        return;
                    }

                    Console.WriteLine($"-> Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room [{removeSensorRequest.RoomId}] removed");

                    SendClientResponse(clientId, XmlResponseFactory.CreateRemoveHeatSensorClientResponse(
                        removeSensorRequest.RoomId, removeSensorRequest.HeatSensorId, true));

                    Console.WriteLine($"-> Remove Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room [{removeSensorRequest.RoomId}] result" +
                                      $" send to client: [{clientId}]");

                    SendHeatSensorRemovedBroadcast(removeSensorRequest.RoomId, removeSensorRequest.HeatSensorId);

                    Console.WriteLine($"-> Removed Heat Sensor [{removeSensorRequest.HeatSensorId}] from Room [{removeSensorRequest.RoomId}] " +
                                      $"Broadcasted to clients");
                }
            }
        }
    }
}
