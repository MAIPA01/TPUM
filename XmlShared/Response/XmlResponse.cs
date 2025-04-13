using System.Xml.Serialization;
using TPUM.XmlShared.Response.Broadcast;
using TPUM.XmlShared.Response.Client;
using TPUM.XmlShared.Response.Subscribe;

namespace TPUM.XmlShared.Response
{
    public enum ResponseType
    {
        Broadcast = 0,
        Client = 1,
        Subscribe = 2
    }

    [XmlRoot("Response")]
    public class Response
    {
        public ResponseType ContentType { get; set; }

        [XmlElement("Broadcast", typeof(BroadcastResponseContent))]
        [XmlElement("Client", typeof(ClientResponseContent))]
        [XmlElement("Subscribe", typeof(SubscribeResponseContent))]
        public required ResponseContent Content { get; set; }
    }

    [XmlInclude(typeof(BroadcastResponseContent))]
    [XmlInclude(typeof(ClientResponseContent))]
    [XmlInclude(typeof(SubscribeResponseContent))]
    public abstract class ResponseContent { }
}
