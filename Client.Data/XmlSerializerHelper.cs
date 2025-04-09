using System.Xml.Serialization;

namespace TPUM.Client.Data
{
    public static class XmlSerializerHelper
    {
        public static string Serialize<T>(T obj)
        {
            using var stringWriter = new StringWriter();
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringWriter, obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
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
    public class HeaterDataContract
    {
        public Guid Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsOn { get; set; }
        public float Temperature { get; set; }
    }

    public class HeatSensorDataContract
    {
        public Guid Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
    }

    public class RoomDataContract
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
    }


    // REQUEST DATA
    [XmlRoot("AllDataRequest")]
    public class AllDataRequest
    {
        public bool WantAll {  get; set; }
    }

    [XmlRoot("AllDataRequestResponse")]
    public class AllDataRequestResponse
    {
        [XmlArray("Rooms")]
        [XmlArrayItem("Room")]
        public List<RoomDataContract> Rooms { get; set; } = [];
    }

    [XmlRoot("RoomDataRequest")]
    public class RoomDataRequest
    {
        public Guid RoomId { get; set; }
    }

    [XmlRoot("RoomDataRequestResponse")]
    public class RoomDataRequestResponse
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
    public class HeaterDataRequest
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
    }

    [XmlRoot("HeaterDataRequestResponse")]
    public class HeaterDataRequestResponse
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
    public class HeatSensorDataRequest
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
    }

    [XmlRoot("HeatSensorDataRequestResponse")]
    public class HeatSensorDataRequestResponse
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
    public class AddRoomRequest
    {
        public required string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }

    [XmlRoot("RoomAddedResponse")]
    public class RoomAddedResponse
    {
        public Guid Id { get; set; }
        public bool Success => Id != Guid.Empty;
    }

    [XmlRoot("AddHeaterRequest")]
    public class AddHeaterRequest
    {
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
    }

    [XmlRoot("HeaterAddedResponse")]
    public class HeaterAddedResponse
    {
        public Guid Id { get; set; }
        public bool Success => Id != Guid.Empty;
    }

    [XmlRoot("AddHeatSensorRequest")]
    public class AddHeatSensorRequest
    {
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }

    [XmlRoot("HeatSensorAddedResponse")]
    public class HeatSensorAddedResponse
    {
        public Guid Id { get; set; }
        public bool Success => Id != Guid.Empty;
    }


    // UPDATE
    [XmlRoot("UpdateRoomRequest")]
    public class UpdateRoomRequest
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }

    [XmlRoot("RoomUpdatedResponse")]
    public class RoomUpdatedResponse
    {
        public bool Success;
    }

    [XmlRoot("UpdateHeaterRequest")]
    public class UpdateHeaterRequest
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Temperature { get; set; }
        public bool IsOn { get; set; }
    }

    [XmlRoot("HeaterUpdatedResponse")]
    public class HeaterUpdatedResponse
    {
        public bool Success;
    }

    [XmlRoot("UpdateHeatSensorRequest")]
    public class UpdateHeatSensorRequest
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }

    [XmlRoot("HeatSensorUpdatedResponse")]
    public class HeatSensorUpdatedResponse
    {
        public bool Success;
    }


    // REMOVE
    [XmlRoot("RemoveRoomRequest")]
    public class RemoveRoomRequest
    {
        public Guid Id { get; set; }
    }

    [XmlRoot("RoomRemovedResponse")]
    public class RoomRemovedResponse
    {
        public bool Success { get; set; }
    }

    [XmlRoot("RemoveHeaterRequest")]
    public class RemoveHeaterRequest
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
    }

    [XmlRoot("HeaterRemovedResponse")]
    public class HeaterRemovedResponse
    {
        public bool Success { get; set; }
    }

    [XmlRoot("RemoveHeatSensorRequest")]
    public class RemoveHeatSensorRequest
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
    }

    [XmlRoot("HeatSensorRemovedResponse")]
    public class HeatSensorRemovedResponse
    {
        public bool Success { get; set; }
    }


    // BROADCAST
    [XmlRoot("RoomBroadcast")]
    public class RoomBroadcast
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
    public class HeaterBroadcast
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
    public class HeatSensorBroadcast
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
