using System.Xml.Serialization;

namespace TPUM.XmlShared.Original.Response.Client
{
#nullable disable
    public enum ClientResponseType
    {
        Get = 0,
        Add = 1,
        Update = 2,
        Remove = 3,
        Subscribe = 4,
        Unsubscribe = 5
    }

    public class ClientResponseContent : ResponseContent
    {
        [XmlAttribute]
        public ClientResponseType DataType { get; set; }

        [XmlElement("GetData", typeof(GetClientResponseData))]
        [XmlElement("AddData", typeof(AddClientResponseData))]
        [XmlElement("UpdateData", typeof(UpdateClientResponseData))]
        [XmlElement("RemoveData", typeof(RemoveClientResponseData))]
        [XmlElement("SubscribeData", typeof(SubscribeClientResponseData))]
        [XmlElement("UnsubscribeData", typeof(UnsubscribeClientResponseData))]
        public ClientResponseData Data { get; set; }
    }

    [XmlInclude(typeof(GetClientResponseData))]
    [XmlInclude(typeof(AddClientResponseData))]
    [XmlInclude(typeof(UpdateClientResponseData))]
    [XmlInclude(typeof(RemoveClientResponseData))]
    [XmlInclude(typeof(SubscribeClientResponseData))]
    [XmlInclude(typeof(UnsubscribeClientResponseData))]
    public abstract class ClientResponseData { }
}