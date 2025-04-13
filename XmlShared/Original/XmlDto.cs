using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TPUM.XmlShared.Dto
{
#nullable disable
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
}
