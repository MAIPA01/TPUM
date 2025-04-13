using System.Xml.Serialization;

namespace TPUM.XmlShared.Response.Subscribe
{
#nullable disable
    public enum SubscribeResponseType
    {
        RoomTemperature = 0
    }

    public class SubscribeResponseContent : ResponseContent
    {
        [XmlAttribute]
        public SubscribeResponseType DataType { get; set; }

        [XmlElement("RoomTemperatureData", typeof(RoomTemperatureSubscribeData))]
        public SubscribeResponseData Data { get; set; }
    }

    [XmlInclude(typeof(RoomTemperatureSubscribeData))]
    public abstract class SubscribeResponseData { }
}
