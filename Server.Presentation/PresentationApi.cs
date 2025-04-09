using Microsoft.VisualBasic;
using TPUM.Server.Logic;

namespace TPUM.Server.Presentation
{
    public abstract class PresentationApiBase : IDisposable
    {
        public abstract Task StartServer();

        public abstract void Dispose();

        public static PresentationApiBase GetApi(string uriPrefix, LogicApiBase? logic = null)
        {
            return new PresentationApi(uriPrefix, logic ?? LogicApiBase.GetApi());
        }
    }

    internal class PresentationApi : PresentationApiBase
    {
        private readonly LogicApiBase _logic;
        private readonly object _roomsLock = new();
        private readonly List<RoomPresentation> _rooms = [];

        private readonly WebSocketServer _server;

        public PresentationApi(string uriPrefix, LogicApiBase logic)
        {
            _logic = logic;
            foreach (var logicRoom in _logic.Rooms)
            {
                var room = new RoomPresentation(logicRoom);
                SubscribeToRoom(room);
                _rooms.Add(room);
            }
            _server = new WebSocketServer(uriPrefix);
            _server.ClientMessageReceived += HandleClientMessage;
        }

        public override Task StartServer()
        {
            return _server.StartAsync();
        }

        private void GetPositionChange(Guid roomId, object? source, IPositionPresentation lastPosition,
            IPositionPresentation newPosition)
        {
            switch (source)
            {
                case IHeaterPresentation heater:
                    BroadcastHeater(roomId, heater.Id, true, false);
                    break;
                case IHeatSensorPresentation sensor:
                    BroadcastHeatSensor(roomId, sensor.Id, true, false);
                    break;
                case IRoomPresentation room:
                    BroadcastRoom(roomId, true, false);
                    break;
            }
        }

        private void GetTemperatureChange(Guid roomId, object? source, float lastTemperature, float newTemperature)
        {
            switch (source)
            {
                case IHeaterPresentation heater:
                    BroadcastHeater(roomId, heater.Id, true, false);
                    break;
                case IHeatSensorPresentation sensor:
                    BroadcastHeatSensor(roomId, sensor.Id, true, false);
                    break;
                case IRoomPresentation room:
                    BroadcastRoom(roomId, true, false);
                    break;
            }
        }

        private void GetEnableChange(Guid roomId, object? source, bool lastEnable, bool newEnable)
        {
            switch (source)
            {
                case IHeaterPresentation heater:
                    BroadcastHeater(roomId, heater.Id, true, false);
                    break;
                case IHeatSensorPresentation sensor:
                    BroadcastHeatSensor(roomId, sensor.Id, true, false);
                    break;
                case IRoomPresentation room:
                    BroadcastRoom(roomId, true, false);
                    break;
            }
        }

        private void SubscribeToRoom(RoomPresentation room)
        {
            room.PositionChanged += GetPositionChange;
            room.TemperatureChanged += GetTemperatureChange;
            room.EnableChanged += GetEnableChange;
        }

        private void UnsubscribeFromRoom(RoomPresentation room)
        {
            room.PositionChanged -= GetPositionChange;
            room.TemperatureChanged -= GetTemperatureChange;
            room.EnableChanged -= GetEnableChange;
        }

        private Guid AddRoom(string name, float width, float height)
        {
            var room = new RoomPresentation(_logic.AddRoom(name, width, height));
            SubscribeToRoom(room);
            lock (_roomsLock)
            {
                _rooms.Add(room);
            }
            return room.Id;
        }

        private bool UpdateRoom(Guid id, string name, float width, float height)
        {
            lock (_roomsLock)
            {
                return GetRoom(id) != null;
            }
        }

        private RoomPresentation? GetRoom(Guid id)
        {
            lock (_roomsLock)
            {
                return _rooms.Find(room => room.Id == id);
            }
        }

        private bool RemoveRoom(Guid id)
        {
            lock (_roomsLock)
            {
                var room = GetRoom(id);
                if (room != null)
                {
                    UnsubscribeFromRoom(room);
                    _rooms.Remove(room);
                }
            }
            _logic.RemoveRoom(id);
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

        private void BroadcastRoom(Guid id, bool updated, bool removed)
        {
            var broadcast = new RoomBroadcast { Id = id, Name = "", Updated = updated, Removed = removed};
            if (!removed)
            {
                var room = GetRoom(id);
                broadcast.Name = room?.Name ?? "";
                broadcast.Height = room?.Height ?? 0;
                broadcast.Width = room?.Width ?? 0;

                foreach (var heater in room?.Heaters ?? [])
                {
                    var heaterData = new HeaterDataContract
                    {
                        Id = heater.Id,
                        IsOn = heater.IsOn,
                        Temperature = heater.Temperature,
                        X = heater.Position.X,
                        Y = heater.Position.Y
                    };
                    broadcast.Heaters.Add(heaterData);
                }

                foreach (var sensor in room?.HeatSensors ?? [])
                {
                    var sensorData = new HeatSensorDataContract
                    {
                        Id = sensor.Id,
                        Temperature = sensor.Temperature,
                        X = sensor.Position.X,
                        Y = sensor.Position.Y
                    };
                    broadcast.HeatSensors.Add(sensorData);
                }
            }

            var message = XmlSerializerHelper.Serialize(broadcast);
            _ = _server.BroadcastAsync(message);
        }

        private Guid AddHeater(Guid roomId, float x, float y, float temperature)
        {
            var room = GetRoom(roomId);
            if (room == null) return Guid.Empty;

            var heater = room.AddHeater(x, y, temperature);
            return heater.Id;
        }

        private bool UpdateHeater(Guid roomId, Guid id, float x, float y, float temperature, bool isOn)
        {
            var room = GetRoom(roomId);
            if (room == null) return false;

            var heater = room.GetHeater(id);
            if (heater == null) return false;

            heater.Position.SetPosition(x, y);
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

        private IHeaterPresentation? GetHeater(Guid roomId, Guid id)
        {
            var room = GetRoom(roomId);
            return room == null ? null : room.GetHeater(id);
        }

        private bool RemoveHeater(Guid roomId, Guid id)
        {
            var room = GetRoom(roomId);
            if (room == null) return true;

            room.RemoveHeater(id);
            return true;
        }

        private void BroadcastHeater(Guid roomId, Guid id, bool updated, bool removed)
        {
            var broadcast = new HeaterBroadcast
            {
                RoomId = roomId,
                Id = id,
                Updated = updated,
                Removed = removed
            };
            if (!removed)
            {
                var room = GetRoom(roomId);
                var heater = room?.GetHeater(id);

                broadcast.X = heater?.Position.X ?? 0;
                broadcast.Y = heater?.Position.Y ?? 0;
                broadcast.IsOn = heater?.IsOn ?? false;
                broadcast.Temperature = heater?.Temperature ?? 0f;
            }

            var message = XmlSerializerHelper.Serialize(broadcast);
            _ = _server.BroadcastAsync(message);
        }

        private Guid AddHeatSensor(Guid roomId, float x, float y)
        {
            var room = GetRoom(roomId);
            if (room == null) return Guid.Empty;

            var sensor = room.AddHeatSensor(x, y);
            return sensor.Id;
        }

        private bool UpdateHeatSensor(Guid roomId, Guid id, float x, float y)
        {
            var room = GetRoom(roomId);
            if (room == null) return false;

            var sensor = room.GetHeatSensor(id);
            if (sensor == null) return false;

            sensor.Position.SetPosition(x, y);
            return true;
        }

        private IHeatSensorPresentation? GetHeatSensor(Guid roomId, Guid id)
        {
            var room = GetRoom(roomId);
            if (room == null) return null;

            return room.GetHeatSensor(id);
        }

        private bool RemoveHeatSensor(Guid roomId, Guid id)
        {
            var room = GetRoom(roomId);
            if (room == null) return true;

            room.RemoveHeatSensor(id);
            return true;
        }

        private void BroadcastHeatSensor(Guid roomId, Guid id, bool updated, bool removed)
        {
            var broadcast = new HeatSensorBroadcast
            {
                RoomId = roomId,
                Id = id,
                Updated = updated,
                Removed = removed
            };
            if (!removed)
            {
                var room = GetRoom(roomId);
                var sensor = room?.GetHeatSensor(id);

                broadcast.X = sensor?.Position.X ?? 0;
                broadcast.Y = sensor?.Position.Y ?? 0;
                broadcast.Temperature = sensor?.Temperature ?? 0f;
            }

            var message = XmlSerializerHelper.Serialize(broadcast);
            _ = _server.BroadcastAsync(message);
        }

        public override void Dispose()
        {
            ClearRooms();
            _logic.ClearRooms();
            GC.SuppressFinalize(this);
        }

        private void HandleClientMessage(object? source, Guid clientId, string message)
        {
            if (XmlSerializerHelper.TryDeserialize<AllDataRequest>(message, out var allDataRequest))
            {
                Console.WriteLine($"Get All Data Request from client: {clientId}");

                var response = new AllDataRequestResponse();
                lock (_roomsLock)
                {
                    foreach (var room in _rooms)
                    {
                        var roomData = new RoomDataContract
                        {
                            Id = room.Id,
                            Name = room.Name,
                            Height = room.Height,
                            Width = room.Width
                        };

                        foreach (var heater in room.Heaters)
                        {
                            var heaterData = new HeaterDataContract
                            {
                                Id = heater.Id,
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
                                Id = sensor.Id,
                                Temperature = sensor.Temperature,
                                X = sensor.Position.X,
                                Y = sensor.Position.Y
                            };
                            roomData.HeatSensors.Add(sensorData);
                        }

                        response.Rooms.Add(roomData);
                    }
                }

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"All Data send to client: {clientId} \n{responseMsg}");
            }
            else if (XmlSerializerHelper.TryDeserialize<RoomDataRequest>(message, out var getRoomRequest))
            {
                Console.WriteLine($"Get Room {getRoomRequest.RoomId} Data Request from client: {clientId}");

                var room = GetRoom(getRoomRequest.RoomId);
                var response = new RoomDataRequestResponse
                {
                    Name = ""
                };
                if (room == null)
                {
                    Console.WriteLine($"Room {getRoomRequest.RoomId} not found");

                    response.Id = Guid.Empty;
                }
                else
                {
                    Console.WriteLine($"Room {getRoomRequest.RoomId} founded");

                    response.Id = room.Id;
                    response.Name = room.Name;
                    response.Width = room.Width;
                    response.Height = room.Height;

                    foreach (var heater in room.Heaters)
                    {
                        var data = new HeaterDataContract
                        {
                            Id = heater.Id,
                            IsOn = heater.IsOn,
                            Temperature = heater.Temperature,
                            X = heater.Position.X,
                            Y = heater.Position.Y
                        };
                        response.Heaters.Add(data);
                    }

                    foreach (var sensor in room.HeatSensors)
                    {
                        var data = new HeatSensorDataContract
                        {
                            Id = sensor.Id,
                            Temperature = sensor.Temperature,
                            X = sensor.Position.X,
                            Y = sensor.Position.Y
                        };
                        response.HeatSensors.Add(data);
                    }
                }

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Room {getRoomRequest.RoomId} data send to client: {clientId}");
            }
            else if (XmlSerializerHelper.TryDeserialize<HeaterDataRequest>(message, out var getHeaterRequest))
            {
                Console.WriteLine($"Get Heater {getHeaterRequest.Id} Data from Room {getHeaterRequest.RoomId} Request from client: {clientId}");

                var response = new HeaterDataRequestResponse();
                var heater = GetHeater(getHeaterRequest.RoomId, getHeaterRequest.Id);
                if (heater == null)
                {
                    Console.WriteLine($"Heater {getHeaterRequest.Id} in Room {getHeaterRequest.RoomId} not found");

                    response.Id = Guid.Empty;
                }
                else
                {
                    Console.WriteLine($"Heater {getHeaterRequest.Id} in Room {getHeaterRequest.RoomId} founded");

                    response.Id = heater.Id;
                    response.RoomId = getHeaterRequest.RoomId;
                    response.IsOn = heater.IsOn;
                    response.Temperature = heater.Temperature;
                    response.X = heater.Position.X;
                    response.Y = heater.Position.Y;
                }

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Heater {getHeaterRequest.Id} data from Room {getHeaterRequest.RoomId} send to client: {clientId}");
            }
            else if (XmlSerializerHelper.TryDeserialize<HeatSensorDataRequest>(message, out var getSensorRequest))
            {
                Console.WriteLine($"Get Heat Sensor {getSensorRequest.Id} Data from Room {getSensorRequest.RoomId} Request from client: {clientId}");

                var response = new HeatSensorDataRequestResponse();
                var sensor = GetHeatSensor(getSensorRequest.RoomId, getSensorRequest.Id);
                if (sensor == null)
                {
                    Console.WriteLine($"Heat Sensor {getSensorRequest.Id} in Room {getSensorRequest.RoomId} not found");

                    response.Id = Guid.Empty;
                }
                else
                {
                    Console.WriteLine($"Heat Sensor {getSensorRequest.Id} in Room {getSensorRequest.RoomId} founded");

                    response.Id = sensor.Id;
                    response.RoomId = getSensorRequest.RoomId;
                    response.Temperature = sensor.Temperature;
                    response.X = sensor.Position.X;
                    response.Y = sensor.Position.Y;
                }

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Heat Sensor {getSensorRequest.Id} Data from Room {getSensorRequest.RoomId} send to client: {clientId}");
            }
            else if (XmlSerializerHelper.TryDeserialize<AddRoomRequest>(message, out var addRoomRequest))
            {
                Console.WriteLine($"Add Room {addRoomRequest.Name} {addRoomRequest.Width}x{addRoomRequest.Height} Request from client: {clientId}");

                var roomId = AddRoom(addRoomRequest.Name, addRoomRequest.Width, addRoomRequest.Height);
                var response = new RoomAddedResponse
                {
                    Id = roomId
                };

                Console.WriteLine(response.Success ? $"Room added with id {roomId}" : "Room not added");

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Add Room Result send to client: {clientId}");

                if (!response.Success) return;

                Console.WriteLine($"Added Room Broadcast to other clients");

                BroadcastRoom(roomId, false, false);
            }
            else if (XmlSerializerHelper.TryDeserialize<AddHeaterRequest>(message, out var addHeaterRequest))
            {
                Console.WriteLine($"Add Heater ({addHeaterRequest.X}, {addHeaterRequest.Y}) {addHeaterRequest.Temperature}\u00b0C " +
                                  $"to Room {addHeaterRequest.RoomId} Request from client: {clientId}");

                var heaterId = AddHeater(addHeaterRequest.RoomId, addHeaterRequest.X, addHeaterRequest.Y,
                    addHeaterRequest.Temperature);
                var response = new HeaterAddedResponse
                {
                    Id = heaterId
                };

                Console.WriteLine(response.Success ? $"Heater added to Room {addHeaterRequest.RoomId} with id {heaterId}" 
                    : $"Heater not added to Room {addHeaterRequest.RoomId}");

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Add Heater to Room {addHeaterRequest.RoomId} result send to client: {clientId}");

                if (!response.Success) return;

                Console.WriteLine($"Added Heater to Room {addHeaterRequest.RoomId} Broadcast to other clients");

                BroadcastHeater(addHeaterRequest.RoomId, heaterId, false, false);
            }
            else if (XmlSerializerHelper.TryDeserialize<AddHeatSensorRequest>(message, out var addSensorRequest))
            {
                Console.WriteLine($"Add Heat Sensor ({addSensorRequest.X}, {addSensorRequest.Y}) to Room {addSensorRequest.RoomId} " +
                                  $"Request from client: {clientId}");

                var sensorId = AddHeatSensor(addSensorRequest.RoomId, addSensorRequest.X, addSensorRequest.Y);
                var response = new HeatSensorAddedResponse
                {
                    Id = sensorId
                };

                Console.WriteLine(response.Success ? $"Heat Sensor added to Room {addSensorRequest.RoomId} with id {sensorId}" 
                    : $"Heat Sensor not added to Room {addSensorRequest.RoomId}");

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Add Heat Sensor to Room {addSensorRequest.RoomId} result send to client: {clientId}");

                if (!response.Success) return;

                Console.WriteLine($"Added Heat Sensor to Room {addSensorRequest.RoomId} Broadcast to other clients");

                BroadcastHeatSensor(addSensorRequest.RoomId, sensorId, false, false);
            }
            else if (XmlSerializerHelper.TryDeserialize<UpdateRoomRequest>(message, out var updateRoomRequest))
            {
                Console.WriteLine($"Update Room {updateRoomRequest.Id} {updateRoomRequest.Name} " +
                                  $"{updateRoomRequest.Width}x{updateRoomRequest.Height} Request from client: {clientId}");

                var response = new RoomUpdatedResponse
                {
                    Success = UpdateRoom(updateRoomRequest.Id, updateRoomRequest.Name, updateRoomRequest.Width,
                        updateRoomRequest.Height)
                };

                Console.WriteLine(response.Success ? $"Room {updateRoomRequest.Id} updated" : $"Room {updateRoomRequest.Id} not updated");

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Update Room {updateRoomRequest.Id} result send to client: {clientId}");

                if (!response.Success) return;

                Console.WriteLine($"Update Room {updateRoomRequest.Id} Broadcast to other clients");

                BroadcastRoom(updateRoomRequest.Id, true, false);
            }
            else if (XmlSerializerHelper.TryDeserialize<UpdateHeaterRequest>(message, out var updateHeaterRequest))
            {
                Console.WriteLine($"Update Heater {updateHeaterRequest.Id} ({updateHeaterRequest.X}, {updateHeaterRequest.Y})" +
                                  $" {updateHeaterRequest.Temperature}\u00b0C On:{updateHeaterRequest.IsOn} in Room " +
                                  $"{getHeaterRequest.RoomId} Request from client: {clientId}");

                var response = new HeaterUpdatedResponse
                {
                    Success = UpdateHeater(updateHeaterRequest.RoomId, updateHeaterRequest.Id, updateHeaterRequest.X, updateHeaterRequest.Y,
                        updateHeaterRequest.Temperature, updateHeaterRequest.IsOn)
                };

                Console.WriteLine(response.Success ? $"Heater {updateHeaterRequest.Id} in Room {updateHeaterRequest.RoomId} updated" 
                    : $"Heater {updateHeaterRequest.Id} in Room {updateHeaterRequest.RoomId} not updated");

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Update Heater {updateHeaterRequest.Id} in Room {updateHeaterRequest.RoomId} result " +
                                  $"send to client: {clientId}");

                if (!response.Success) return;

                Console.WriteLine($"Updated Heater {updateHeaterRequest.Id} from Room {updateHeaterRequest.RoomId} Broadcast to other clients");

                BroadcastHeater(updateHeaterRequest.RoomId, updateHeaterRequest.Id, true, false);
            }
            else if (XmlSerializerHelper.TryDeserialize<UpdateHeatSensorRequest>(message, out var updateSensorRequest))
            {
                Console.WriteLine($"Update Heat Sensor {updateSensorRequest.Id} ({updateSensorRequest.X}, {updateSensorRequest.Y})" +
                                  $" in Room {updateSensorRequest.RoomId} Request from client: {clientId}");

                var response = new HeatSensorUpdatedResponse
                {
                    Success = UpdateHeatSensor(updateSensorRequest.RoomId, updateSensorRequest.Id, updateSensorRequest.X, updateSensorRequest.Y)
                };

                Console.WriteLine(response.Success ? $"Heat Sensor {updateSensorRequest.Id} in Room {updateSensorRequest.RoomId} updated"
                    : $"Heat Sensor {updateSensorRequest.Id} in Room {updateSensorRequest.RoomId} not updated");

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Update Heat Sensor {updateSensorRequest.Id} in Room {updateSensorRequest.RoomId} result " +
                                  $"send to client: {clientId}");

                if (!response.Success) return;

                Console.WriteLine($"Updated Heat Sensor {updateSensorRequest.Id} from Room {updateSensorRequest.RoomId} " +
                                  $"Broadcast to other clients");

                BroadcastHeatSensor(updateSensorRequest.RoomId, updateSensorRequest.Id, true, false);
            }
            else if (XmlSerializerHelper.TryDeserialize<RemoveRoomRequest>(message, out var removeRoomRequest))
            {
                Console.WriteLine($"Remove Room {removeRoomRequest.Id} Request from client: {clientId}");

                var response = new RoomRemovedResponse
                {
                    Success = RemoveRoom(removeRoomRequest.Id)
                };

                Console.WriteLine(response.Success ? $"Room {removeRoomRequest.Id} removed" : $"Room {removeRoomRequest.Id} not removed");

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Remove Room {removeRoomRequest.Id} result send to client: {clientId}");

                if (!response.Success) return;

                Console.WriteLine($"Removed Room {removeRoomRequest.Id} Broadcast to other clients");

                BroadcastRoom(removeRoomRequest.Id, false, true);
            }
            else if (XmlSerializerHelper.TryDeserialize<RemoveHeaterRequest>(message, out var removeHeaterRequest))
            {
                Console.WriteLine($"Remove Heater {removeHeaterRequest.Id} from Room {removeHeaterRequest.RoomId} " +
                                  $"Request from client: {clientId}");

                var response = new HeaterRemovedResponse
                {
                    Success = RemoveHeater(removeHeaterRequest.RoomId, removeHeaterRequest.Id)
                };

                Console.WriteLine(response.Success ? $"Heater {removeHeaterRequest.Id} from Room {removeHeaterRequest.RoomId} removed" 
                    : $"Heater {removeHeaterRequest.Id} from Room {removeHeaterRequest.RoomId} not removed");

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Remove Heater {removeHeaterRequest.Id} from Room {removeHeaterRequest.RoomId} result " +
                                  $"send to client: {clientId}");

                if (!response.Success) return;

                Console.WriteLine($"Removed Heater {removeHeaterRequest.Id} from Room {removeHeaterRequest.RoomId} Broadcast to other clients");

                BroadcastHeater(removeHeaterRequest.RoomId, removeHeaterRequest.Id, false, true);
            }
            else if (XmlSerializerHelper.TryDeserialize<RemoveHeatSensorRequest>(message, out var removeSensorRequest))
            {
                Console.WriteLine($"Remove Heat Sensor {removeSensorRequest.Id} from Room {removeSensorRequest.RoomId} " +
                                  $"Request from client: {clientId}");

                var response = new HeatSensorRemovedResponse
                {
                    Success = RemoveHeatSensor(removeSensorRequest.RoomId, removeSensorRequest.Id)
                };

                Console.WriteLine(response.Success ? $"Heat Sensor {removeSensorRequest.Id} from Room {removeSensorRequest.RoomId} removed" 
                    : $"Heat Sensor {removeSensorRequest.Id} from Room {removeSensorRequest.RoomId} not removed");

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                Console.WriteLine($"Remove Heat Sensor {removeSensorRequest.Id} from Room {removeSensorRequest.RoomId} result" +
                                  $" send to client: {clientId}");

                if (!response.Success) return;

                Console.WriteLine($"Removed Heat Sensor {removeSensorRequest.Id} from Room {removeSensorRequest.RoomId} " +
                                  $"Broadcast to other clients");

                BroadcastHeatSensor(removeSensorRequest.RoomId, removeSensorRequest.Id, false, true);
            }
        }
    }
}
