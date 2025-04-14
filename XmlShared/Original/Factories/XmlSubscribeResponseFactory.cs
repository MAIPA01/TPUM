using TPUM.XmlShared.Original.Response.Subscribe;

namespace TPUM.XmlShared.Original.Response.Factory
{
    public static class XmlSubscribeResponseFactory
    {
        private static Response CreateSubscribeResponse(SubscribeResponseType type, SubscribeResponseData data)
        {
            return XmlResponseFactory.CreateResponse(ResponseType.Subscribe, new SubscribeResponseContent
            {
                DataType = type,
                Data = data
            });
        }

        public static Response CreateRoomTemperatureSubscribeResponse(Guid roomId, Guid sensorId, float temperature)
        {
            return CreateSubscribeResponse(SubscribeResponseType.RoomTemperature, new RoomTemperatureSubscribeData
            {
                RoomId = roomId,
                HeatSensorId = sensorId,
                Temperature = temperature
            });
        }
    }
}