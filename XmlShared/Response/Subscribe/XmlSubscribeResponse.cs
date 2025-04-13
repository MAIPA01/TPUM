using System.Xml.Serialization;

namespace TPUM.XmlShared.Response.Subscribe
{
    public enum SubscribeResponseType
    {
        RoomTemperature = 0
    }

    public class SubscribeResponseContent : ResponseContent
    {
        public SubscribeResponseType DataType { get; set; }

        [XmlElement("RoomTemperatureData", typeof(RoomTemperatureSubscribeData))]
        public required SubscribeResponseData Data { get; set; }
    }

    [XmlInclude(typeof(RoomTemperatureSubscribeData))]
    public abstract class SubscribeResponseData { }
}
