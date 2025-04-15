using TPUM.XmlShared.Original.Response.Broadcast;

namespace TPUM.XmlShared.Original.Response.Factory
{
    internal static class XmlBroadcastResponseFactory
    {
        private static Response CreateBroadcastResponse(BroadcastResponseType type, BroadcastResponseData data)
        {
            return XmlResponseFactory.CreateResponse(ResponseType.Broadcast, new BroadcastResponseContent
            {
                BroadcastType = type,
                Broadcast = data
            });
        }

        #region ADD_BROADCAST_RESPONSES
        private static Response CreateAddBroadcastResponse(AddBroadcastType type, AddBroadcastData data)
        {
            return CreateBroadcastResponse(BroadcastResponseType.Add, new AddBroadcastResponse
            {
                DataType = type,
                Data = data
            });
        }

        public static Response CreateAddRoomBroadcastResponse(Guid roomId, string name, float width, float height)
        {
            return CreateAddBroadcastResponse(AddBroadcastType.Room, new AddRoomBroadcastData
            {
                RoomId = roomId,
                Name = name,
                Width = width,
                Height = height
            });
        }

        public static Response CreateAddHeaterBroadcastResponse(Guid roomId, Guid heaterId, float x, float y, float temperature)
        {
            return CreateAddBroadcastResponse(AddBroadcastType.Heater, new AddHeaterBroadcastData
            {
                RoomId = roomId,
                HeaterId = heaterId,
                X = x,
                Y = y,
                Temperature = temperature
            });
        }

        public static Response CreateAddHeatSensorBroadcastResponse(Guid roomId, Guid sensorId, float x, float y, float temperature)
        {
            return CreateAddBroadcastResponse(AddBroadcastType.HeatSensor, new AddHeatSensorBroadcastData
            {
                RoomId = roomId,
                HeatSensorId = sensorId,
                X = x,
                Y = y,
                Temperature = temperature
            });
        }
        #endregion ADD_BROADCAST_RESPONSES

        #region UPDATE_BROADCAST_RESPONSES
        private static Response CreateUpdateBroadcastResponse(UpdateBroadcastType type, UpdateBroadcastData data)
        {
            return CreateBroadcastResponse(BroadcastResponseType.Update, new UpdateBroadcastResponse
            {
                DataType = type,
                Data = data
            });
        }

        public static Response CreateUpdateHeaterBroadcastResponse(Guid roomId, Guid heaterId, float x, float y, float temperature,
            bool isOn)
        {
            return CreateUpdateBroadcastResponse(UpdateBroadcastType.Heater, new UpdateHeaterBroadcastData
            {
                RoomId = roomId,
                HeaterId = heaterId,
                X = x,
                Y = y,
                Temperature = temperature,
                IsOn = isOn
            });
        }

        public static Response CreateUpdateHeatSensorBroadcastResponse(Guid roomId, Guid sensorId, float x, float y, float temperature)
        {
            return CreateUpdateBroadcastResponse(UpdateBroadcastType.HeatSensor, new UpdateHeatSensorBroadcastData
            {
                RoomId = roomId,
                HeatSensorId = sensorId,
                X = x,
                Y = y,
                Temperature = temperature
            });
        }
        #endregion UPDATE_BROADCAST_RESPONSES

        #region REMOVE_BROADCAST_RESPONSES
        private static Response CreateRemoveBroadcastResponse(RemoveBroadcastType type, RemoveBroadcastData data)
        {
            return CreateBroadcastResponse(BroadcastResponseType.Remove, new RemoveBroadcastResponse
            {
                DataType = type,
                Data = data
            });
        }

        public static Response CreateRemoveRoomBroadcastResponse(Guid roomId)
        {
            return CreateRemoveBroadcastResponse(RemoveBroadcastType.Room, new RemoveRoomBroadcastData
            {
                RoomId = roomId
            });
        }

        public static Response CreateRemoveHeaterBroadcastResponse(Guid roomId, Guid heaterId)
        {
            return CreateRemoveBroadcastResponse(RemoveBroadcastType.Heater, new RemoveHeaterBroadcastData
            {
                RoomId = roomId,
                HeaterId = heaterId
            });
        }

        public static Response CreateRemoveHeatSensorBroadcastResponse(Guid roomId, Guid sensorId)
        {
            return CreateRemoveBroadcastResponse(RemoveBroadcastType.HeatSensor, new RemoveHeatSensorBroadcastData
            {
                RoomId = roomId,
                HeatSensorId = sensorId
            });
        }
        #endregion REMOVE_BROADCAST_RESPONSES
    }
}