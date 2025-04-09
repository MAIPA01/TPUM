using System.Xml.Serialization;

namespace TPUM.Client.Data
{
    internal static class XmlSerializerHelper
    {
        public static string Serialize<T>(T obj)
        {
            using var stringWriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }

        public static bool TryDeserialize<T>(string xml, out T result)
        {
            try
            {
                result = Deserialize<T>(xml);
                return true;
            }
            catch
            {
                result = default!;
                return false;
            }
        }

        public static T Deserialize<T>(string xml)
        {
            using var stringReader = new StringReader(xml);
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stringReader)!;
        }
    }


    // DTO
    internal class HeaterDataContract
    {
        public Guid Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsOn { get; set; }
        public float Temperature { get; set; }
    }

    internal class HeatSensorDataContract
    {
        public Guid Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
    }


    // REQUEST DATA
    [XmlRoot("RoomDataRequest")]
    internal class RoomDataRequest
    {
        public Guid RoomId { get; set; }
    }

    [XmlRoot("RoomDataRequestResponse")]
    internal class RoomDataRequestResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public bool NotFound => Id == Guid.Empty;

        [XmlArray("Heaters")]
        [XmlArrayItem("Heater")]
        public List<HeaterDataContract> Heaters { get; set; } = [];

        [XmlArray("HeatSensors")]
        [XmlArrayItem("HeatSensor")]
        public List<HeatSensorDataContract> HeatSensors { get; set; } = [];
    }

    [XmlRoot("HeaterDataRequest")]
    internal class HeaterDataRequest
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
    }

    [XmlRoot("HeaterDataRequestResponse")]
    internal class HeaterDataRequestResponse
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsOn { get; set; }
        public float Temperature { get; set; }

        public bool NotFound => Id == Guid.Empty;
    }

    [XmlRoot("HeatSensorDataRequest")]
    internal class HeatSensorDataRequest
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
    }

    [XmlRoot("HeatSensorDataRequestResponse")]
    internal class HeatSensorDataRequestResponse
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }

        public bool NotFound => Id == Guid.Empty;
    }


    // ADD
    [XmlRoot("AddRoomRequest")]
    internal class AddRoomRequest
    {
        public required string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }

    [XmlRoot("RoomAddedResponse")]
    internal class RoomAddedResponse
    {
        public Guid Id { get; set; }
        public bool Success => Id != Guid.Empty;
    }

    [XmlRoot("AddHeaterRequest")]
    internal class AddHeaterRequest
    {
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
    }

    [XmlRoot("HeaterAddedResponse")]
    internal class HeaterAddedResponse
    {
        public Guid Id { get; set; }
        public bool Success => Id != Guid.Empty;
    }

    [XmlRoot("AddHeatSensorRequest")]
    internal class AddHeatSensorRequest
    {
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }

    [XmlRoot("HeatSensorAddedResponse")]
    internal class HeatSensorAddedResponse
    {
        public Guid Id { get; set; }
        public bool Success => Id != Guid.Empty;
    }


    // REMOVE
    [XmlRoot("RemoveRoomRequest")]
    internal class RemoveRoomRequest
    {
        public Guid Id { get; set; }
    }

    [XmlRoot("RoomRemovedResponse")]
    internal class RoomRemovedResponse
    {
        public bool Success { get; set; }
    }

    [XmlRoot("RemoveHeaterRequest")]
    internal class RemoveHeaterRequest
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
    }

    [XmlRoot("HeaterRemovedResponse")]
    internal class HeaterRemovedResponse
    {
        public bool Success { get; set; }
    }

    [XmlRoot("RemoveHeatSensorRequest")]
    internal class RemoveHeatSensorRequest
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
    }

    [XmlRoot("HeatSensorRemovedResponse")]
    internal class HeatSensorRemovedResponse
    {
        public bool Success { get; set; }
    }


    // BROADCAST
    [XmlRoot("RoomBroadcast")]
    internal class RoomBroadcast
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        [XmlArray("Heaters")]
        [XmlArrayItem("Heater")]
        public List<HeaterDataContract> Heaters { get; set; } = [];

        [XmlArray("HeatSensors")]
        [XmlArrayItem("HeatSensor")]
        public List<HeatSensorDataContract> HeatSensors { get; set; } = [];

        public bool Updated { get; set; }
        public bool Removed { get; set; }
    }

    [XmlRoot("HeaterBroadcast")]
    internal class HeaterBroadcast
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsOn { get; set; }
        public float Temperature { get; set; }

        public bool Updated { get; set; }
        public bool Removed { get; set; }
    }

    [XmlRoot("HeatSensorBroadcast")]
    internal class HeatSensorBroadcast
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }

        public bool Updated { get; set; }
        public bool Removed { get; set; }
    }
}
