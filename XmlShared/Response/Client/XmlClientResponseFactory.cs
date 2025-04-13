namespace TPUM.XmlShared.Response.Client
{
    public static class XmlClientResponseFactory
    {
        private static Response CreateClientResponse(ClientResponseType type, ClientResponseData data)
        {
            return XmlResponseFactory.CreateResponse(ResponseType.Client, new ClientResponseContent
            {
                DataType = type,
                Data = data
            });
        }

        #region GET_CLIENT_RESPONSES
        private static Response CreateGetClientResponse(GetClientType type, GetClientData data)
        {
            return CreateClientResponse(ClientResponseType.Get, new GetClientResponseData
            {
                DataType = type,
                Data = data
            });
        }

        public static Response CreateGetAllClientResponse(List<RoomDataContract> roomsDto)
        {
            return CreateGetClientResponse(GetClientType.All, new GetAllClientData
            {
                Rooms = roomsDto
            });
        }

        public static Response CreateGetRoomSuccessClientResponse(Guid roomId, string name, float height, float width,
            List<HeaterDataContract> heaters, List<HeatSensorDataContract> sensors)
        {
            return CreateGetClientResponse(GetClientType.Room, new GetRoomClientData
            {
                RoomId = roomId,
                Result = new GetRoomClientDataResult
                {
                    Name = name,
                    Height = height,
                    Width = width,
                    Heaters = heaters,
                    HeatSensors = sensors
                }
            });
        }

        public static Response CreateGetRoomFailedClientResponse(Guid roomId)
        {
            return CreateGetClientResponse(GetClientType.Room, new GetRoomClientData
            {
                RoomId = roomId,
                Result = null
            });
        }

        public static Response CreateGetHeaterSuccessClientResponse(Guid roomId, Guid heaterId, float x, float y, float temperature,
            bool isOn)
        {
            return CreateGetClientResponse(GetClientType.Heater, new GetHeaterClientData
            {
                RoomId = roomId,
                HeaterId = heaterId,
                Result = new GetHeaterClientDataResult
                {
                    X = x,
                    Y = y,
                    Temperature = temperature,
                    IsOn = isOn
                }
            });
        }

        public static Response CreateGetHeaterFailedClientResponse(Guid roomId, Guid heaterId)
        {
            return CreateGetClientResponse(GetClientType.Heater, new GetHeaterClientData
            {
                RoomId = roomId,
                HeaterId = heaterId,
                Result = null
            });
        }

        public static Response CreateGetHeatSensorSuccessClientResponse(Guid roomId, Guid sensorId, float x, float y, float temperature)
        {
            return CreateGetClientResponse(GetClientType.HeatSensor, new GetHeatSensorClientData
            {
                RoomId = roomId,
                HeatSensorId = sensorId,
                Result = new GetHeatSensorClientDataResult
                {
                    X = x,
                    Y = y,
                    Temperature = temperature
                }
            });
        }

        public static Response CreateGetHeatSensorFailedClientResponse(Guid roomId, Guid sensorId)
        {
            return CreateGetClientResponse(GetClientType.HeatSensor, new GetHeatSensorClientData
            {
                RoomId = roomId,
                HeatSensorId = sensorId,
                Result = null
            });
        }
        #endregion GET_CLIENT_RESPONSES

        #region ADD_CLIENT_RESPONSES

        private static Response CreateAddSuccessClientResponse(AddClientType type, AddClientData data)
        {
            return CreateClientResponse(ClientResponseType.Add, new AddClientResponseData
            {
                DataType = type,
                Success = true,
                Data = data
            });
        }

        private static Response CreateAddFailedClientResponse(AddClientType type)
        {
            return CreateClientResponse(ClientResponseType.Add, new AddClientResponseData
            {
                DataType = type,
                Success = false,
                Data = null
            });
        }

        public static Response CreateAddRoomSuccessClientResponse(Guid roomId)
        {
            return CreateAddSuccessClientResponse(AddClientType.Room, new AddRoomClientData
            {
                RoomId = roomId
            });
        }

        public static Response CreateAddRoomFailedClientResponse()
        {
            return CreateAddFailedClientResponse(AddClientType.Room);
        }

        public static Response CreateAddHeaterSuccessClientResponse(Guid roomId, Guid heaterId)
        {
            return CreateAddSuccessClientResponse(AddClientType.Heater, new AddHeaterClientData
            {
                RoomId = roomId,
                HeaterId = heaterId
            });
        }

        public static Response CreateAddHeaterFailedClientResponse()
        {
            return CreateAddFailedClientResponse(AddClientType.Heater);
        }

        public static Response CreateAddHeatSensorSuccessClientResponse(Guid roomId, Guid heatSensorId)
        {
            return CreateAddSuccessClientResponse(AddClientType.HeatSensor, new AddHeatSensorClientData
            {
                RoomId = roomId,
                HeatSensorId = heatSensorId
            });
        }

        public static Response CreateAddHeatSensorFailedClientResponse()
        {
            return CreateAddFailedClientResponse(AddClientType.HeatSensor);
        }
        #endregion ADD_CLIENT_RESPONSES

        #region UPDATE_CLIENT_RESPONES
        private static Response CreateUpdateClientResponse(UpdateClientType type, bool success, UpdateClientData data)
        {
            return CreateClientResponse(ClientResponseType.Update, new UpdateClientResponseData
            {
                DataType = type,
                Success = success,
                Data = data
            });
        }

        public static Response CreateUpdateHeaterClientResponse(Guid roomId, Guid heaterId, bool success)
        {
            return CreateUpdateClientResponse(UpdateClientType.Heater, success, new UpdateHeaterClientData
            {
                RoomId = roomId,
                HeaterId = heaterId
            });
        }

        public static Response CreateUpdateHeatSensorClientResponse(Guid roomId, Guid sensorId, bool success)
        {
            return CreateUpdateClientResponse(UpdateClientType.HeatSensor, success, new UpdateHeatSensorClientData
            {
                RoomId = roomId,
                HeatSensorId = sensorId
            });
        }
        #endregion UPDATE_CLIENT_RESPONES

        #region REMOVE_CLIENT_RESPONSES
        private static Response CreateRemoveClientResponse(RemoveClientType type, bool success, RemoveClientData data)
        {
            return CreateClientResponse(ClientResponseType.Remove, new RemoveClientResponseData
            {
                DataType = type,
                Success = success,
                Data = data
            });
        }

        public static Response CreateRemoveRoomClientResponse(Guid roomId, bool success)
        {
            return CreateRemoveClientResponse(RemoveClientType.Room, success, new RemoveRoomClientData
            {
                RoomId = roomId
            });
        }

        public static Response CreateRemoveHeaterClientResponse(Guid roomId, Guid heaterId, bool success)
        {
            return CreateRemoveClientResponse(RemoveClientType.Heater, success, new RemoveHeaterClientData
            {
                RoomId = roomId,
                HeaterId = heaterId
            });
        }

        public static Response CreateRemoveHeatSensorClientResponse(Guid roomId, Guid sensorId, bool success)
        {
            return CreateRemoveClientResponse(RemoveClientType.HeatSensor, success, new RemoveHeatSensorClientData
            {
                RoomId = roomId,
                HeatSensorId = sensorId
            });
        }
        #endregion REMOVE_CLIENT_RESPONSES

        #region SUBSCRIBE_CLIENT_RESPONSES
        private static Response CreateSubscribeClientResponse(SubscribeClientType type, bool success, SubscribeClientData data)
        {
            return CreateClientResponse(ClientResponseType.Subscribe, new SubscribeClientResponseData
            {
                DataType = type,
                Success = success,
                Data = data
            });
        }

        public static Response CreateSubscribeRoomTemperatureClientResponse(Guid roomId, bool success)
        {
            return CreateSubscribeClientResponse(SubscribeClientType.RoomTemperature, success,
                new SubscribeRoomTemperatureClientData
                {
                    RoomId = roomId
                });
        }
        #endregion

        #region UNSUBSCRIBE_CLIENT_RESPONSES
        private static Response CreateUnsubscribeClientResponse(UnsubscribeClientType type, bool success, UnsubscribeClientData data)
        {
            return CreateClientResponse(ClientResponseType.Unsubscribe, new UnsubscribeClientResponseData
            {
                DataType = type,
                Success = success,
                Data = data
            });
        }

        public static Response CreateUnsubscribeRoomTemperatureClientResponse(Guid roomId, bool success)
        {
            return CreateUnsubscribeClientResponse(UnsubscribeClientType.RoomTemperature, success,
                new UnsubscribeRoomTemperatureClientData
                {
                    RoomId = roomId
                });
        }
        #endregion
    }
}
