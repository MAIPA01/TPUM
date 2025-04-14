using System.Xml.Serialization;
using TPUM.XmlShared.Original.Response.Broadcast;
using TPUM.XmlShared.Original.Response.Client;
using TPUM.XmlShared.Original.Response.Subscribe;

namespace TPUM.XmlShared.Original.Response
{
#nullable disable
    public enum ResponseType
    {
        Broadcast = 0,
        Client = 1,
        Subscribe = 2
    }

    [XmlRoot("Response")]
    public class Response
    {
        [XmlAttribute]
        public ResponseType ContentType { get; set; }

        [XmlElement("Broadcast", typeof(BroadcastResponseContent))]
        [XmlElement("Client", typeof(ClientResponseContent))]
        [XmlElement("Subscribe", typeof(SubscribeResponseContent))]
        public ResponseContent Content { get; set; }
    }

    [XmlInclude(typeof(BroadcastResponseContent))]
    [XmlInclude(typeof(ClientResponseContent))]
    [XmlInclude(typeof(SubscribeResponseContent))]
    public abstract class ResponseContent { }
}