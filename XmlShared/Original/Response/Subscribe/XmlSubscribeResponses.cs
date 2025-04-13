using System;

namespace TPUM.XmlShared.Response.Subscribe
{
    public class RoomTemperatureSubscribeData : SubscribeResponseData
    {
        public Guid RoomId { get; set; }
        public Guid HeatSensorId { get; set; }
        public float Temperature { get; set; }
    }
}
