using System;
using System.Xml.Serialization;

namespace TPUM.XmlShared.Request
{
    public enum RequestType
    {
        Get = 0,
        Add = 1,
        Update = 2,
        Remove = 3,
        Subscribe = 4,
        Unsubscribe = 5
    }

    [XmlRoot("Request")]
    public class Request
    {
        [XmlAttribute]
        public RequestType ContentType { get; set; }

        [XmlElement("GetContent", typeof(GetRequestContent))]
        [XmlElement("AddContent", typeof(AddRequestContent))]
        [XmlElement("UpdateContent", typeof(UpdateRequestContent))]
        [XmlElement("RemoveContent", typeof(RemoveRequestContent))]
        [XmlElement("SubscribeContent", typeof(SubscribeRequestContent))]
        [XmlElement("UnsubscribeContent", typeof(UnsubscribeRequestContent))]
#nullable disable
        public RequestContent Content { get; set; }
#nullable restore
    }

    [XmlInclude(typeof(GetRequestContent))]
    [XmlInclude(typeof(AddRequestContent))]
    [XmlInclude(typeof(UpdateRequestContent))]
    [XmlInclude(typeof(RemoveRequestContent))]
    [XmlInclude(typeof(SubscribeRequestContent))]
    [XmlInclude(typeof(UnsubscribeRequestContent))]
    public abstract class RequestContent { }
}
