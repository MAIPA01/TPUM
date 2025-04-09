using TPUM.Server.Logic;

namespace TPUM.Server.Presentation
{
    public abstract class PresentationApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoomPresentation> Rooms { get; }

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
        private readonly List<IRoomPresentation> _rooms = [];
        public override IReadOnlyCollection<IRoomPresentation> Rooms => _rooms.AsReadOnly();

        private readonly WebSocketServer _server;

        public PresentationApi(string uriPrefix, LogicApiBase logic)
        {
            _logic = logic;
            foreach (var room in _logic.Rooms)
            {
                _rooms.Add(new RoomPresentation(room));
            }
            _server = new WebSocketServer(uriPrefix);
            _server.ClientMessageReceived += HandleClientMessage;
        }

        public override Task StartServer()
        {
            return _server.StartAsync();
        }

        private Guid AddRoom(string name, float width, float height)
        {
            var room = new RoomPresentation(_logic.AddRoom(name, width, height));
            _rooms.Add(room);
            return room.Id;
        }

        private bool UpdateRoom(Guid id, string name, float width, float height)
        {
            var room = _rooms.Find(room => room.Id == id);
            return room != null;
        }

        private IRoomPresentation? GetRoom(Guid id)
        {
            return _rooms.Find(room => room.Id == id);
        }

        private bool RemoveRoom(Guid id)
        {
            var room = _rooms.Find(room => room.Id == id);
            if (room != null)
            {
                _rooms.Remove(room);
            }
            _logic.RemoveRoom(id);
            return true;
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
            var room = _rooms.Find(room => room.Id == roomId);
            if (room == null) return Guid.Empty;

            var heater = room.AddHeater(x, y, temperature);
            return heater.Id;
        }

        private bool UpdateHeater(Guid roomId, Guid id, float x, float y, float temperature, bool isOn)
        {
            var room = _rooms.Find(room => room.Id == roomId);
            if (room == null) return false;

            var heater = room.GetHeater(id);
            if (heater == null) return false;

            heater.Position.X = x;
            heater.Position.Y = y;
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
            var room = _rooms.Find(room => room.Id == roomId);
            return room == null ? null : room.GetHeater(id);
        }

        private bool RemoveHeater(Guid roomId, Guid id)
        {
            var room = _rooms.Find(room => room.Id == roomId);
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
            var room = _rooms.Find(room => room.Id == roomId);
            if (room == null) return Guid.Empty;

            var sensor = room.AddHeatSensor(x, y);
            return sensor.Id;
        }

        private bool UpdateHeatSensor(Guid roomId, Guid id, float x, float y)
        {
            var room = _rooms.Find(room => room.Id == roomId);
            if (room == null) return false;

            var sensor = room.GetHeatSensor(id);
            if (sensor == null) return false;

            sensor.Position.X = x;
            sensor.Position.Y = y;
            return true;
        }

        private IHeatSensorPresentation? GetHeatSensor(Guid roomId, Guid id)
        {
            var room = _rooms.Find(room => room.Id == roomId);
            if (room == null) return null;

            return room.GetHeatSensor(id);
        }

        private bool RemoveHeatSensor(Guid roomId, Guid id)
        {
            var room = _rooms.Find(room => room.Id == roomId);
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
            foreach (var room in _rooms)
            {
                room.Dispose();
            }
            _rooms.Clear();
            GC.SuppressFinalize(this);
        }

        private void HandleClientMessage(object? source, Guid clientId, string message)
        {
            if (XmlSerializerHelper.TryDeserialize<RoomDataRequest>(message, out var getRoomRequest))
            {
                var room = GetRoom(getRoomRequest.RoomId);
                var response = new RoomDataRequestResponse
                {
                    Name = ""
                };
                if (room == null)
                {
                    response.Id = Guid.Empty;
                }
                else
                {
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
            }
            else if (XmlSerializerHelper.TryDeserialize<HeaterDataRequest>(message, out var getHeaterRequest))
            {
                var response = new HeaterDataRequestResponse();
                var heater = GetHeater(getHeaterRequest.RoomId, getHeaterRequest.Id);
                if (heater == null)
                {
                    response.Id = Guid.Empty;
                }
                else
                {
                    response.Id = heater.Id;
                    response.RoomId = getHeaterRequest.RoomId;
                    response.IsOn = heater.IsOn;
                    response.Temperature = heater.Temperature;
                    response.X = heater.Position.X;
                    response.Y = heater.Position.Y;
                }

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);
            }
            else if (XmlSerializerHelper.TryDeserialize<HeatSensorDataRequest>(message, out var getSensorRequest))
            {
                var response = new HeatSensorDataRequestResponse();
                var sensor = GetHeatSensor(getSensorRequest.RoomId, getSensorRequest.Id);
                if (sensor == null)
                {
                    response.Id = Guid.Empty;
                }
                else
                {
                    response.Id = sensor.Id;
                    response.RoomId = getSensorRequest.RoomId;
                    response.Temperature = sensor.Temperature;
                    response.X = sensor.Position.X;
                    response.Y = sensor.Position.Y;
                }

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);
            }
            else if (XmlSerializerHelper.TryDeserialize<AddRoomRequest>(message, out var addRoomRequest))
            {
                var roomId = AddRoom(addRoomRequest.Name, addRoomRequest.Width, addRoomRequest.Height);
                var response = new RoomAddedResponse
                {
                    Id = roomId
                };

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                if (response.Success)
                {
                    BroadcastRoom(roomId, false, false);
                }
            }
            else if (XmlSerializerHelper.TryDeserialize<AddHeaterRequest>(message, out var addHeaterRequest))
            {
                var heaterId = AddHeater(addHeaterRequest.RoomId, addHeaterRequest.X, addHeaterRequest.Y,
                    addHeaterRequest.Temperature);
                var response = new HeaterAddedResponse
                {
                    Id = heaterId
                };

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                if (response.Success)
                {
                    BroadcastHeater(addHeaterRequest.RoomId, heaterId, false, false);
                }
            }
            else if (XmlSerializerHelper.TryDeserialize<AddHeatSensorRequest>(message, out var addSensorRequest))
            {
                var sensorId = AddHeatSensor(addSensorRequest.RoomId, addSensorRequest.X, addSensorRequest.Y);
                var response = new HeatSensorAddedResponse
                {
                    Id = sensorId
                };

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                if (response.Success)
                {
                    BroadcastHeatSensor(addSensorRequest.RoomId, sensorId, false, false);
                }
            }
            else if (XmlSerializerHelper.TryDeserialize<UpdateRoomRequest>(message, out var updateRoomRequest))
            {
                var response = new RoomUpdatedResponse
                {
                    Success = UpdateRoom(updateRoomRequest.Id, updateRoomRequest.Name, updateRoomRequest.Width,
                        updateRoomRequest.Height)
                };

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                if (response.Success)
                {
                    BroadcastRoom(updateRoomRequest.Id, true, false);
                }
            }
            else if (XmlSerializerHelper.TryDeserialize<UpdateHeaterRequest>(message, out var updateHeaterRequest))
            {
                var response = new HeaterUpdatedResponse
                {
                    Success = UpdateHeater(updateHeaterRequest.RoomId, updateHeaterRequest.Id, updateHeaterRequest.X, updateHeaterRequest.Y,
                        updateHeaterRequest.Temperature, updateHeaterRequest.IsOn)
                };

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                if (response.Success)
                {
                    BroadcastHeater(updateHeaterRequest.RoomId, updateHeaterRequest.Id, true, false);
                }
            }
            else if (XmlSerializerHelper.TryDeserialize<UpdateHeatSensorRequest>(message, out var updateSensorRequest))
            {
                var response = new HeatSensorUpdatedResponse
                {
                    Success = UpdateHeatSensor(updateSensorRequest.RoomId, updateSensorRequest.Id, updateSensorRequest.X, updateSensorRequest.Y)
                };

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                if (response.Success)
                {
                    BroadcastHeatSensor(updateSensorRequest.RoomId, updateSensorRequest.Id, true, false);
                }
            }
            else if (XmlSerializerHelper.TryDeserialize<RemoveRoomRequest>(message, out var removeRoomRequest))
            {
                var response = new RoomRemovedResponse
                {
                    Success = RemoveRoom(removeRoomRequest.Id)
                };

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                if (response.Success)
                {
                    BroadcastRoom(removeRoomRequest.Id, false, true);
                }
            }
            else if (XmlSerializerHelper.TryDeserialize<RemoveHeaterRequest>(message, out var removeHeaterRequest))
            {
                var response = new HeaterRemovedResponse
                {
                    Success = RemoveHeater(removeHeaterRequest.RoomId, removeHeaterRequest.Id)
                };

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                if (response.Success)
                {
                    BroadcastHeater(removeHeaterRequest.RoomId, removeHeaterRequest.Id, false, true);
                }
            }
            else if (XmlSerializerHelper.TryDeserialize<RemoveHeatSensorRequest>(message, out var removeSensorRequest))
            {
                var response = new HeatSensorRemovedResponse
                {
                    Success = RemoveHeatSensor(removeSensorRequest.RoomId, removeSensorRequest.Id)
                };

                var responseMsg = XmlSerializerHelper.Serialize(response);
                _ = _server.SendAsync(clientId, responseMsg);

                if (response.Success)
                {
                    BroadcastHeatSensor(removeSensorRequest.RoomId, removeSensorRequest.Id, false, true);
                }
            }
        }
    }
}
