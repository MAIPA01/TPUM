using TPUM.XmlShared.Generated;

namespace TPUM.Server.Presentation.Factory
{
    internal static class XmlSubscribeResponseFactory
    {
        public static Response CreateRoomTemperatureSubscribeResponse(Guid roomId, Guid sensorId, float temperature)
        {
            return XmlResponseFactory.CreateResponse(ResponseType.Subscribe, new SubscribeResponseContent
            {
                DataType = SubscribeResponseType.RoomTemperature,
                RoomTemperatureData = new RoomTemperatureSubscribeData
                {
                    RoomId = roomId,
                    HeatSensorId = sensorId,
                    Temperature = temperature
                }
            });
        }
    }
}