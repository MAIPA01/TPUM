using System.Xml.Serialization;
using TPUM.XmlShared.Original.Dto;

namespace TPUM.XmlShared.Original.Response.Client
{
#nullable disable
    #region GET_CLIENT_RESPONSE
    public enum GetClientType
    {
        All = 0,
        Room = 1,
        Heater = 2,
        HeatSensor = 3
    }

    public class GetClientResponseData : ClientResponseData
    {
        [XmlAttribute]
        public GetClientType DataType { get; set; }

        [XmlElement("AllData", typeof(GetAllClientData))]
        [XmlElement("RoomData", typeof(GetRoomClientData))]
        [XmlElement("HeaterData", typeof(GetHeaterClientData))]
        [XmlElement("HeatSensorData", typeof(GetHeatSensorClientData))]
        public GetClientData Data { get; set; }
    }

    [XmlInclude(typeof(GetAllClientData))]
    [XmlInclude(typeof(GetRoomClientData))]
    [XmlInclude(typeof(GetHeaterClientData))]
    [XmlInclude(typeof(GetHeatSensorClientData))]
    public abstract class GetClientData { }

    public class GetAllClientData : GetClientData
    {
        [XmlArray("Rooms")]
        [XmlArrayItem("Room")]
        public List<RoomDataContract> Rooms { get; set; } = [];
    }

    public class GetRoomClientData : GetClientData
    {
        public Guid RoomId { get; set; }

#nullable enable
        public GetRoomClientDataResult? Result { get; set; } = null;
#nullable disable

        [XmlIgnore]
        public bool Success => Result != null;
    }

    public class GetRoomClientDataResult
    {
        public string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        [XmlArray("Heaters")]
        [XmlArrayItem("Heater")]
        public List<HeaterDataContract> Heaters { get; set; } = [];

        [XmlArray("HeatSensors")]
        [XmlArrayItem("HeatSensor")]
        public List<HeatSensorDataContract> HeatSensors { get; set; } = [];
    }

    public class GetHeaterClientData : GetClientData
    {
        public Guid RoomId { get; set; }

        public Guid HeaterId { get; set; }

#nullable enable
        public GetHeaterClientDataResult? Result { get; set; } = null;
#nullable disable

        [XmlIgnore]
        public bool Success => Result != null;
    }

    public class GetHeaterClientDataResult
    {
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsOn { get; set; }
        public float Temperature { get; set; }
    }

    public class GetHeatSensorClientData : GetClientData
    {
        public Guid RoomId { get; set; }
        public Guid HeatSensorId { get; set; }

#nullable enable
        public GetHeatSensorClientDataResult? Result { get; set; } = null;
#nullable disable

        [XmlIgnore]
        public bool Success => Result != null;
    }

    public class GetHeatSensorClientDataResult
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
    }
    #endregion GET_CLIENT_RESPONSE

    #region ADD_CLIENT_RESPONSE
    public enum AddClientType
    {
        Room = 0,
        Heater = 1,
        HeatSensor = 2
    }

    public class AddClientResponseData : ClientResponseData
    {
        [XmlAttribute]
        public AddClientType DataType { get; set; }

        public bool Success { get; set; }

        [XmlElement("RoomData", typeof(AddRoomClientData))]
        [XmlElement("HeaterData", typeof(AddHeaterClientData))]
        [XmlElement("HeatSensorData", typeof(AddHeatSensorClientData))]
#nullable enable
        public AddClientData? Data { get; set; }
#nullable disable
    }

    [XmlInclude(typeof(AddRoomClientData))]
    [XmlInclude(typeof(AddHeaterClientData))]
    [XmlInclude(typeof(AddHeatSensorClientData))]
    public abstract class AddClientData { }

    public class AddRoomClientData : AddClientData
    {
        public Guid RoomId;
    }

    public class AddHeaterClientData : AddClientData
    {
        public Guid RoomId;
        public Guid HeaterId;
    }

    public class AddHeatSensorClientData : AddClientData
    {
        public Guid RoomId;
        public Guid HeatSensorId;
    }
    #endregion ADD_CLIENT_RESPONSE

    #region UPDATE_CLIENT_RESPONSE
    public enum UpdateClientType
    {
        Heater = 0,
        HeatSensor = 1
    }

    public class UpdateClientResponseData : ClientResponseData
    {
        [XmlAttribute]
        public UpdateClientType DataType { get; set; }

        public bool Success { get; set; }

        [XmlElement("HeaterData", typeof(UpdateHeaterClientData))]
        [XmlElement("HeatSensorData", typeof(UpdateHeatSensorClientData))]
        public UpdateClientData Data { get; set; }
    }

    [XmlInclude(typeof(UpdateHeaterClientData))]
    [XmlInclude(typeof(UpdateHeatSensorClientData))]
    public abstract class UpdateClientData { }

    public class UpdateHeaterClientData : UpdateClientData
    {
        public Guid RoomId;
        public Guid HeaterId;
    }

    public class UpdateHeatSensorClientData : UpdateClientData
    {
        public Guid RoomId;
        public Guid HeatSensorId;
    }
    #endregion UPDATE_CLIENT_RESPONSE

    #region REMOVE_CLIENT_RESPONSE
    public enum RemoveClientType
    {
        Room = 0,
        Heater = 1,
        HeatSensor = 2
    }

    public class RemoveClientResponseData : ClientResponseData
    {
        [XmlAttribute]
        public RemoveClientType DataType { get; set; }

        public bool Success { get; set; }

        [XmlElement("RoomData", typeof(RemoveRoomClientData))]
        [XmlElement("HeaterData", typeof(RemoveHeaterClientData))]
        [XmlElement("HeatSensorData", typeof(RemoveHeatSensorClientData))]
        public RemoveClientData Data { get; set; }
    }

    [XmlInclude(typeof(RemoveRoomClientData))]
    [XmlInclude(typeof(RemoveHeaterClientData))]
    [XmlInclude(typeof(RemoveHeatSensorClientData))]
    public abstract class RemoveClientData { }

    public class RemoveRoomClientData : RemoveClientData
    {
        public Guid RoomId { get; set; }
    }

    public class RemoveHeaterClientData : RemoveClientData
    {
        public Guid RoomId { get; set; }
        public Guid HeaterId { get; set; }
    }

    public class RemoveHeatSensorClientData : RemoveClientData
    {
        public Guid RoomId { get; set; }
        public Guid HeatSensorId { get; set; }
    }
    #endregion REMOVE_CLIENT_RESPONSE

    #region SUBSCRIBE_CLIENT_RESPONSE
    public enum SubscribeClientType
    {
        RoomTemperature = 0
    }

    public class SubscribeClientResponseData : ClientResponseData
    {
        [XmlAttribute]
        public SubscribeClientType DataType { get; set; }

        public bool Success { get; set; }

        [XmlElement("RoomTemperatureData", typeof(SubscribeRoomTemperatureClientData))]
        public SubscribeClientData Data { get; set; }
    }

    [XmlInclude(typeof(SubscribeRoomTemperatureClientData))]
    public abstract class SubscribeClientData { }

    public class SubscribeRoomTemperatureClientData : SubscribeClientData
    {
        public Guid RoomId { get; set; }
    }
    #endregion

    #region UNSUBSCRIBE_CLIENT_RESPONSE
    public enum UnsubscribeClientType
    {
        RoomTemperature = 0
    }

    public class UnsubscribeClientResponseData : ClientResponseData
    {
        [XmlAttribute]
        public UnsubscribeClientType DataType { get; set; }

        public bool Success { get; set; }

        [XmlElement("RoomTemperatureData", typeof(UnsubscribeRoomTemperatureClientData))]
        public UnsubscribeClientData Data { get; set; }
    }

    [XmlInclude(typeof(UnsubscribeRoomTemperatureClientData))]
    public abstract class UnsubscribeClientData { }

    public class UnsubscribeRoomTemperatureClientData : UnsubscribeClientData
    {
        public Guid RoomId { get; set; }
    }
    #endregion
}