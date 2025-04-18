﻿using System.Xml.Serialization;

namespace TPUM.XmlShared.Original.Response.Broadcast
{
#nullable disable
    public enum BroadcastResponseType
    {
        Add = 0,
        Update = 1,
        Remove = 2
    }

    public class BroadcastResponseContent : ResponseContent
    {
        [XmlAttribute]
        public BroadcastResponseType BroadcastType { get; set; }

        [XmlElement("AddBroadcast", typeof(AddBroadcastResponse))]
        [XmlElement("UpdateBroadcast", typeof(UpdateBroadcastResponse))]
        [XmlElement("RemoveBroadcast", typeof(RemoveBroadcastResponse))]
        public BroadcastResponseData Broadcast { get; set; }
    }

    [XmlInclude(typeof(AddBroadcastResponse))]
    [XmlInclude(typeof(UpdateBroadcastResponse))]
    [XmlInclude(typeof(RemoveBroadcastResponse))]
    public abstract class BroadcastResponseData { }
}