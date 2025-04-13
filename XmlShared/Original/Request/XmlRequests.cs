using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace TPUM.XmlShared.Request
{
    #region GET_REQUEST
    public enum GetRequestType
    {
        All = 0,
        Room = 1,
        Heater = 2,
        HeatSensor = 3
    }

    public class GetRequestContent : RequestContent
    {
        [XmlAttribute]
        public GetRequestType DataType { get; set; }

        [XmlElement("RoomData", typeof(GetRoomRequestData))]
        [XmlElement("HeaterData", typeof(GetHeaterRequestData))]
        [XmlElement("HeatSensorData", typeof(GetHeatSensorRequestData))]
#nullable enable
        public GetRequestData? Data { get; set; }
#nullable disable
    }

    [XmlInclude(typeof(GetRoomRequestData))]
    [XmlInclude(typeof(GetHeaterRequestData))]
    [XmlInclude(typeof(GetHeatSensorRequestData))]
    public abstract class GetRequestData { }

    public class GetRoomRequestData : GetRequestData
    {
        public Guid RoomId { get; set; }
    }

    public class GetHeaterRequestData : GetRequestData
    {
        public Guid RoomId { get; set; }
        public Guid HeaterId { get; set; }
    }

    public class GetHeatSensorRequestData : GetRequestData
    {
        public Guid RoomId { get; set; }
        public Guid HeatSensorId { get; set; }
    }
    #endregion GET_REQUEST

    #region ADD_REQUEST
    public enum AddRequestType
    {
        Room = 0,
        Heater = 1,
        HeatSensor = 2
    }

    public class AddRequestContent : RequestContent
    {
        [XmlAttribute]
        public AddRequestType DataType { get; set; }

        [XmlElement("RoomData", typeof(AddRoomRequestData))]
        [XmlElement("HeaterData", typeof(AddHeaterRequestData))]
        [XmlElement("HeatSensorData", typeof(AddHeatSensorRequestData))]
        public AddRequestData Data { get; set; }
    }

    [XmlInclude(typeof(AddRoomRequestData))]
    [XmlInclude(typeof(AddHeaterRequestData))]
    [XmlInclude(typeof(AddHeatSensorRequestData))]
    public abstract class AddRequestData { }

    public class AddRoomRequestData : AddRequestData
    {
        public string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }

    public class AddHeaterRequestData : AddRequestData
    {
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
    }

    public class AddHeatSensorRequestData : AddRequestData
    {
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
    #endregion ADD_REQUEST

    #region UPDATE_REQUEST
    public enum UpdateRequestType
    {
        Heater = 0,
        HeatSensor = 1
    }

    public class UpdateRequestContent : RequestContent
    {
        [XmlAttribute]
        public UpdateRequestType DataType { get; set; }

        [XmlElement("HeaterData", typeof(UpdateHeaterRequestData))]
        [XmlElement("HeatSensorData", typeof(UpdateHeatSensorRequestData))]
        public UpdateRequestData Data { get; set; }
    }

    [XmlInclude(typeof(UpdateHeaterRequestData))]
    [XmlInclude(typeof(UpdateHeatSensorRequestData))]
    public abstract class UpdateRequestData { }

    public class UpdateHeaterRequestData : UpdateRequestData
    {
        public Guid RoomId { get; set; }
        public Guid HeaterId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
        public bool IsOn { get; set; }
    }

    public class UpdateHeatSensorRequestData : UpdateRequestData
    {
        public Guid RoomId { get; set; }
        public Guid HeatSensorId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
    #endregion UPDATE_REQUEST

    #region REMOVE_REQUEST
    public enum RemoveRequestType
    {
        Room = 0,
        Heater = 1,
        HeatSensor = 2
    }

    public class RemoveRequestContent : RequestContent
    {
        [XmlAttribute]
        public RemoveRequestType DataType { get; set; }

        [XmlElement("RoomData", typeof(RemoveRoomRequestData))]
        [XmlElement("HeaterData", typeof(RemoveHeaterRequestData))]
        [XmlElement("HeatSensorData", typeof(RemoveHeatSensorRequestData))]
        public RemoveRequestData Data { get; set; }
    }

    [XmlInclude(typeof(RemoveRoomRequestData))]
    [XmlInclude(typeof(RemoveHeaterRequestData))]
    [XmlInclude(typeof(RemoveHeatSensorRequestData))]
    public abstract class RemoveRequestData { }

    public class RemoveRoomRequestData : RemoveRequestData
    {
        public Guid RoomId { get; set; }
    }

    public class RemoveHeaterRequestData : RemoveRequestData
    {
        public Guid RoomId { get; set; }
        public Guid HeaterId { get; set; }
    }

    public class RemoveHeatSensorRequestData : RemoveRequestData
    {
        public Guid RoomId { get; set; }
        public Guid HeatSensorId { get; set; }
    }
    #endregion REMOVE_REQUEST

    #region SUBSCRIBE_REQUEST
    public enum SubscribeRequestType
    {
        RoomTemperature = 0
    }

    public class SubscribeRequestContent : RequestContent
    {
        [XmlAttribute]
        public SubscribeRequestType DataType { get; set; }

        [XmlElement("RoomTemperatureData", typeof(SubscribeRoomTemperatureRequestData))]
        public SubscribeRequestData Data { get; set; }
    }

    [XmlInclude(typeof(SubscribeRoomTemperatureRequestData))]
    public abstract class SubscribeRequestData {}

    public class SubscribeRoomTemperatureRequestData : SubscribeRequestData
    {
        public Guid RoomId { get; set; }
    }
    #endregion SUBSCRIBE_REQUEST

    #region UNSUBSCRIBE_REQUEST
    public enum UnsubscribeRequestType
    {
        RoomTemperature = 0
    }

    public class UnsubscribeRequestContent : RequestContent
    {
        [XmlAttribute]
        public UnsubscribeRequestType DataType { get; set; }

        [XmlElement("RoomTemperatureData", typeof(UnsubscribeRoomTemperatureRequestData))]
        public UnsubscribeRequestData Data { get; set; }
    }

    [XmlInclude(typeof(UnsubscribeRoomTemperatureRequestData))]
    public abstract class UnsubscribeRequestData { }

    public class UnsubscribeRoomTemperatureRequestData : UnsubscribeRequestData
    {
        public Guid RoomId { get; set; }
    }

    #endregion UNSUBSCRIBE_REQUEST
}
