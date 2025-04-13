using System.Xml.Serialization;

namespace TPUM.XmlShared.Response.Broadcast
{
    #region ADD_BROADCAST_RESPONSE
    public enum AddBroadcastType
    {
        Room = 0,
        Heater = 1,
        HeatSensor = 2
    }

    public class AddBroadcastResponse : BroadcastResponseData
    {
        public AddBroadcastType DataType { get; set; }

        [XmlElement("RoomData", typeof(AddRoomBroadcastData))]
        [XmlElement("HeaterData", typeof(AddHeaterBroadcastData))]
        [XmlElement("HeatSensorData", typeof(AddHeatSensorBroadcastData))]
        public required AddBroadcastData Data { get; set; }
    }

    [XmlInclude(typeof(AddRoomBroadcastData))]
    [XmlInclude(typeof(AddHeaterBroadcastData))]
    [XmlInclude(typeof(AddHeatSensorBroadcastData))]
    public abstract class AddBroadcastData { }

    public class AddRoomBroadcastData : AddBroadcastData
    {
        public Guid RoomId { get; set; }
        public string Name { get; set; } = "Temp Name";
        public float Width { get; set; }
        public float Height { get; set; }
    }

    public class AddHeaterBroadcastData : AddBroadcastData
    {
        public Guid RoomId { get; set; }
        public Guid HeaterId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
    }

    public class AddHeatSensorBroadcastData : AddBroadcastData
    {
        public Guid RoomId { get; set; }
        public Guid HeatSensorId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
    }
    #endregion ADD_BROADCAST_RESPONSE

    #region UPDATE_BROADCAST_RESPONSE
    public enum UpdateBroadcastType
    {
        Heater = 0,
        HeatSensor = 1
    }

    public class UpdateBroadcastResponse : BroadcastResponseData
    {
        public UpdateBroadcastType DataType { get; set; }

        [XmlElement("HeaterData", typeof(UpdateHeaterBroadcastData))]
        [XmlElement("HeatSensorData", typeof(UpdateHeatSensorBroadcastData))]
        public required UpdateBroadcastData Data { get; set; }
    }

    [XmlInclude(typeof(UpdateHeaterBroadcastData))]
    [XmlInclude(typeof(UpdateHeatSensorBroadcastData))]
    public abstract class UpdateBroadcastData { }

    public class UpdateHeaterBroadcastData : UpdateBroadcastData
    {
        public Guid RoomId { get; set; }
        public Guid HeaterId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsOn { get; set; }
        public float Temperature { get; set; }
    }

    public class UpdateHeatSensorBroadcastData : UpdateBroadcastData
    {
        public Guid RoomId { get; set; }
        public Guid HeatSensorId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
    }
    #endregion UPDATE_BROADCAST_RESPONSE

    #region REMOVE_BROADCAST_RESPONSE
    public enum RemoveBroadcastType
    {
        Room = 0,
        Heater = 1,
        HeatSensor = 2
    }

    public class RemoveBroadcastResponse : BroadcastResponseData
    {
        public RemoveBroadcastType DataType { get; set; }

        [XmlElement("RoomData", typeof(RemoveRoomBroadcastData))]
        [XmlElement("HeaterData", typeof(RemoveHeaterBroadcastData))]
        [XmlElement("HeatSensorData", typeof(RemoveHeatSensorBroadcastData))]
        public required RemoveBroadcastData Data { get; set; }
    }

    [XmlInclude(typeof(RemoveRoomBroadcastData))]
    [XmlInclude(typeof(RemoveHeaterBroadcastData))]
    [XmlInclude(typeof(RemoveHeatSensorBroadcastData))]
    public abstract class RemoveBroadcastData { }

    public class RemoveRoomBroadcastData : RemoveBroadcastData
    {
        public Guid RoomId { get; set; }
    }

    public class RemoveHeaterBroadcastData : RemoveBroadcastData
    {
        public Guid RoomId { get; set; }
        public Guid HeaterId { get; set; }
    }

    public class RemoveHeatSensorBroadcastData : RemoveBroadcastData
    {
        public Guid RoomId { get; set; }
        public Guid HeatSensorId { get; set; }
    }
    #endregion REMOVE_BROADCAST_RESPONSE
}
