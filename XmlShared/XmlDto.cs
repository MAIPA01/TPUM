using System.Xml.Serialization;

namespace TPUM.XmlShared
{
    #region DTO
    public class HeaterDataContract
    {
        public Guid HeaterId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsOn { get; set; }
        public float Temperature { get; set; }
    }

    public class HeatSensorDataContract
    {
        public Guid HeatSensorId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
    }

    public class RoomDataContract
    {
        public Guid RoomId { get; set; }
        public string Name { get; set; } = "Temp Name";
        public float Width { get; set; }
        public float Height { get; set; }

        [XmlArray("Heaters")]
        [XmlArrayItem("Heater")]
        public List<HeaterDataContract> Heaters { get; set; } = [];

        [XmlArray("HeatSensors")]
        [XmlArrayItem("HeatSensor")]
        public List<HeatSensorDataContract> HeatSensors { get; set; } = [];
    }
    #endregion DTO
}
