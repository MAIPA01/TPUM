namespace TPUM.XmlShared.Generated.Factory
{
    public static class XmlRequestFactory
    {
        private static Request CreateRequest(RequestType type, RequestContent content)
        {
            return new Request
            {
                ContentType = type,
                Item = content
            };
        }

        #region GET_REQUESTS
        private static Request CreateGetRequest(GetRequestType type, GetRequestData? data)
        {
            return CreateRequest(RequestType.Get, new GetRequestContent
            {
                DataType = type,
                Item = data
            });
        }

        public static Request CreateGetAllDataRequest()
        {
            return CreateGetRequest(GetRequestType.All, null);
        }

        public static Request CreateGetRoomDataRequest(Guid roomId)
        {
            return CreateGetRequest(GetRequestType.Room, new GetRoomRequestData
            {
                RoomId = roomId
            });
        }

        public static Request CreateGetHeaterDataRequest(Guid roomId, Guid heaterId)
        {
            return CreateGetRequest(GetRequestType.Heater, new GetHeaterRequestData
            {
                RoomId = roomId,
                HeaterId = heaterId
            });
        }

        public static Request CreateGetHeatSensorDataRequest(Guid roomId, Guid heatSensorId)
        {
            return CreateGetRequest(GetRequestType.HeatSensor, new GetHeatSensorRequestData
            {
                RoomId = roomId,
                HeatSensorId = heatSensorId
            });
        }
        #endregion GET_REQUESTS

        #region ADD_REQUESTS
        private static Request CreateAddRequest(AddRequestType type, AddRequestData data)
        {
            return CreateRequest(RequestType.Add, new AddRequestContent
            {
                DataType = type,
                Item = data
            });
        }

        public static Request CreateAddRoomRequest(string name, float width, float height)
        {
            return CreateAddRequest(AddRequestType.Room, new AddRoomRequestData
            {
                Name = name,
                Width = width,
                Height = height
            });
        }

        public static Request CreateAddHeaterRequest(Guid roomId, float x, float y, float temperature)
        {
            return CreateAddRequest(AddRequestType.Heater, new AddHeaterRequestData
            {
                RoomId = roomId,
                X = x,
                Y = y,
                Temperature = temperature
            });
        }

        public static Request CreateAddHeatSensorRequest(Guid roomId, float x, float y)
        {
            return CreateAddRequest(AddRequestType.HeatSensor, new AddHeatSensorRequestData
            {
                RoomId = roomId,
                X = x,
                Y = y
            });
        }
        #endregion ADD_REQUESTS

        #region UPDATE_REQUESTS
        private static Request CreateUpdateRequest(UpdateRequestType type, UpdateRequestData data)
        {
            return CreateRequest(RequestType.Update, new UpdateRequestContent
            {
                DataType = type,
                Item = data
            });
        }

        public static Request CreateUpdateHeaterRequest(Guid roomId, Guid heaterId, float x, float y, float temperature, bool isOn)
        {
            return CreateUpdateRequest(UpdateRequestType.Heater, new UpdateHeaterRequestData
            {
                RoomId = roomId,
                HeaterId = heaterId,
                X = x,
                Y = y,
                Temperature = temperature,
                IsOn = isOn
            });
        }

        public static Request CreateUpdateHeatSensorRequest(Guid roomId, Guid sensorId, float x, float y)
        {
            return CreateUpdateRequest(UpdateRequestType.HeatSensor, new UpdateHeatSensorRequestData
            {
                RoomId = roomId,
                HeatSensorId = sensorId,
                X = x,
                Y = y
            });
        }
        #endregion UPDATE_REQUESTS

        #region REMOVE_REQUESTS
        private static Request CreateRemoveRequest(RemoveRequestType type, RemoveRequestData data)
        {
            return CreateRequest(RequestType.Remove, new RemoveRequestContent
            {
                DataType = type,
                Item = data
            });
        }

        public static Request CreateRemoveRoomRequest(Guid roomId)
        {
            return CreateRemoveRequest(RemoveRequestType.Room, new RemoveRoomRequestData
            {
                RoomId = roomId
            });
        }

        public static Request CreateRemoveHeaterRequest(Guid roomId, Guid heaterId)
        {
            return CreateRemoveRequest(RemoveRequestType.Heater, new RemoveHeaterRequestData
            {
                RoomId = roomId,
                HeaterId = heaterId
            });
        }

        public static Request CreateRemoveHeatSensorRequest(Guid roomId, Guid heatSensorId)
        {
            return CreateRemoveRequest(RemoveRequestType.HeatSensor, new RemoveHeatSensorRequestData
            {
                RoomId = roomId,
                HeatSensorId = heatSensorId
            });
        }
        #endregion REMOVE_REQUESTS

        #region SUBSCRIBE_REQUESTS
        public static Request CreateSubscribeRoomTemperatureRequest(Guid roomId)
        {
            return CreateRequest(RequestType.Subscribe, new SubscribeRequestContent
            {
                DataType = SubscribeRequestType.RoomTemperature,
                RoomTemperatureData = new SubscribeRoomTemperatureRequestData
                {
                    RoomId = roomId
                }
            });
        }
        #endregion SUBSCRIBE_REQUESTS

        #region UNSUBSCRIBE_REQUESTS
        public static Request CreateUnsubscribeRoomTemperatureRequest(Guid roomId)
        {
            return CreateRequest(RequestType.Unsubscribe, new UnsubscribeRequestContent
            {
                DataType = UnsubscribeRequestType.RoomTemperature,
                RoomTemperatureData = new UnsubscribeRoomTemperatureRequestData
                {
                    RoomId = roomId
                }
            });
        }
        #endregion UNSUBSCRIBE_REQUESTS
    }
}
