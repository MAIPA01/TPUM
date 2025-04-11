using System.Xml.Serialization;
using TPUM.Client.Data.Events;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TPUM.Client.Data
{
    internal static class XmlSerializerHelper
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

    internal static class XmlDtoFactory
    {
        public static RoomDataContract CreateRoomDto(Guid roomId, string name, float width, float height, List<HeaterDataContract> heaters,
            List<HeatSensorDataContract> sensors)
        {
            return new RoomDataContract
            {
                RoomId = roomId,
                Name = name,
                Width = width,
                Height = height,
                Heaters = heaters,
                HeatSensors = sensors
            };
        }

        public static HeaterDataContract CreateHeaterDto(Guid heaterId, float x, float y, float temperature, bool isOn)
        {
            return new HeaterDataContract
            {
                HeaterId = heaterId,
                X = x,
                Y = y,
                Temperature = temperature,
                IsOn = isOn
            };
        }

        public static HeatSensorDataContract CreateHeatSensorDto(Guid sensorId, float x, float y, float temperature)
        {
            return new HeatSensorDataContract
            {
                HeatSensorId = sensorId,
                X = x,
                Y = y,
                Temperature = temperature
            };
        }
    }

    internal static class XmlRequestFactory
    {
        private static Request CreateRequest(RequestType type, RequestContent content)
        {
            return new Request
            {
                ContentType = type,
                Content = content
            };
        }

        #region GET_REQUESTS
        private static Request CreateGetRequest(GetRequestType type, GetRequestData data)
        {
            return CreateRequest(RequestType.Get, new GetRequestContent
            {
                DataType = type,
                Data = data
            });
        }

        public static Request CreateGetAllDataRequest()
        {
            return CreateGetRequest(GetRequestType.All, new GetAllRequestData());
        }

        public static Request CreateGetRoomDataRequest(Guid roomId)
        {
            return CreateGetRequest(GetRequestType.Room, new GetRoomRequestData
            {
                RoomId = roomId
            });
        }

        public static Request CreateGetHeaterDataRequest(Guid roomId, Guid heaterId)
        {
            return CreateGetRequest(GetRequestType.Heater, new GetHeaterRequestData
            {
                RoomId = roomId,
                HeaterId = heaterId
            });
        }

        public static Request CreateGetHeatSensorDataRequest(Guid roomId, Guid heatSensorId)
        {
            return CreateGetRequest(GetRequestType.HeatSensor, new GetHeatSensorRequestData
            {
                RoomId = roomId,
                HeatSensorId = heatSensorId
            });
        }
        #endregion GET_REQUESTS

        #region ADD_REQUESTS
        private static Request CreateAddRequest(AddRequestType type, AddRequestData data)
        {
            return CreateRequest(RequestType.Add, new AddRequestContent
            {
                DataType = type,
                Data = data
            });
        }

        public static Request CreateAddRoomRequest(string name, float width, float height)
        {
            return CreateAddRequest(AddRequestType.Room, new AddRoomRequestData
            {
                Name = name,
                Width = width,
                Height = height
            });
        }

        public static Request CreateAddHeaterRequest(Guid roomId, float x, float y, float temperature)
        {
            return CreateAddRequest(AddRequestType.Heater, new AddHeaterRequestData
            {
                RoomId = roomId,
                X = x,
                Y = y,
                Temperature = temperature
            });
        }

        public static Request CreateAddHeatSensorRequest(Guid roomId, float x, float y)
        {
            return CreateAddRequest(AddRequestType.HeatSensor, new AddHeatSensorRequestData
            {
                RoomId = roomId,
                X = x,
                Y = y
            });
        }
        #endregion ADD_REQUESTS

        #region UPDATE_REQUESTS
        private static Request CreateUpdateRequest(UpdateRequestType type, UpdateRequestData data)
        {
            return CreateRequest(RequestType.Update, new UpdateRequestContent
            {
                DataType = type,
                Data = data
            });
        }

        public static Request CreateUpdateHeaterRequest(Guid roomId, Guid heaterId, float x, float y, float temperature, bool isOn)
        {
            return CreateUpdateRequest(UpdateRequestType.Heater, new UpdateHeaterRequestData
            {
                RoomId = roomId,
                HeaterId = heaterId,
                X = x,
                Y = y,
                Temperature = temperature,
                IsOn = isOn
            });
        }

        public static Request CreateUpdateHeatSensorRequest(Guid roomId, Guid sensorId, float x, float y)
        {
            return CreateUpdateRequest(UpdateRequestType.HeatSensor, new UpdateHeatSensorRequestData
            {
                RoomId = roomId,
                HeatSensorId = sensorId,
                X = x,
                Y = y
            });
        }
        #endregion UPDATE_REQUESTS

        #region REMOVE_REQUESTS
        private static Request CreateRemoveRequest(RemoveRequestType type, RemoveRequestData data)
        {
            return CreateRequest(RequestType.Remove, new RemoveRequestContent
            {
                DataType = type,
                Data = data
            });
        }

        public static Request CreateRemoveRoomRequest(Guid roomId)
        {
            return CreateRemoveRequest(RemoveRequestType.Room, new RemoveRoomRequestData
            {
                RoomId = roomId
            });
        }

        public static Request CreateRemoveHeaterRequest(Guid roomId, Guid heaterId)
        {
            return CreateRemoveRequest(RemoveRequestType.Heater, new RemoveHeaterRequestData
            {
                RoomId = roomId,
                HeaterId = heaterId
            });
        }

        public static Request CreateRemoveHeatSensorRequest(Guid roomId, Guid heatSensorId)
        {
            return CreateRemoveRequest(RemoveRequestType.HeatSensor, new RemoveHeatSensorRequestData
            {
                RoomId = roomId,
                HeatSensorId = heatSensorId
            });
        }
        #endregion REMOVE_REQUESTS
    }

    internal static class XmlResponseFactory
    {
        private static Response CreateResponse(ResponseType type, ResponseContent content)
        {
            return new Response
            {
                ContentType = type,
                Content = content
            };
        }

        #region BROADCAST_RESPONSES
        private static Response CreateBroadcastResponse(BroadcastResponseType type, BroadcastResponseData data)
        {
            return CreateResponse(ResponseType.Broadcast, new BroadcastResponseContent
            {
                BroadcastType = type,
                Broadcast = data
            });
        }

        #region ADD_BROADCAST_RESPONSES
        private static Response CreateAddBroadcastResponse(AddBroadcastType type, AddBroadcastData data)
        {
            return CreateBroadcastResponse(BroadcastResponseType.Add, new AddBroadcastResponse
            {
                DataType = type,
                Data = data
            });
        }

        public static Response CreateAddRoomBroadcastResponse(Guid roomId, string name, float width, float height)
        {
            return CreateAddBroadcastResponse(AddBroadcastType.Room, new AddRoomBroadcastData
            {
                RoomId = roomId,
                Name = name,
                Width = width,
                Height = height
            });
        }

        public static Response CreateAddHeaterBroadcastResponse(Guid roomId, Guid heaterId, float x, float y, float temperature)
        {
            return CreateAddBroadcastResponse(AddBroadcastType.Heater, new AddHeaterBroadcastData
            {
                RoomId = roomId,
                HeaterId = heaterId,
                X = x,
                Y = y,
                Temperature = temperature
            });
        }

        public static Response CreateAddHeatSensorBroadcastResponse(Guid roomId, Guid sensorId, float x, float y)
        {
            return CreateAddBroadcastResponse(AddBroadcastType.HeatSensor, new AddHeatSensorBroadcastData
            {
                RoomId = roomId,
                HeatSensorId = sensorId,
                X = x,
                Y = y
            });
        }
        #endregion ADD_BROADCAST_RESPONSES

        #region UPDATE_BROADCAST_RESPONSES
        private static Response CreateUpdateBroadcastResponse(UpdateBroadcastType type, UpdateBroadcastData data)
        {
            return CreateBroadcastResponse(BroadcastResponseType.Update, new UpdateBroadcastResponse
            {
                DataType = type,
                Data = data
            });
        }

        public static Response CreateUpdateHeaterBroadcastResponse(Guid roomId, Guid heaterId, float x, float y, float temperature, 
            bool isOn)
        {
            return CreateUpdateBroadcastResponse(UpdateBroadcastType.Heater, new UpdateHeaterBroadcastData
            {
                RoomId = roomId,
                HeaterId = heaterId,
                X = x,
                Y = y,
                Temperature = temperature,
                IsOn = isOn
            });
        }

        public static Response CreateUpdateHeatSensorBroadcastResponse(Guid roomId, Guid sensorId, float x, float y, float temperature)
        {
            return CreateUpdateBroadcastResponse(UpdateBroadcastType.HeatSensor, new UpdateHeatSensorBroadcastData
            {
                RoomId = roomId,
                HeatSensorId = sensorId,
                X = x,
                Y = y,
                Temperature = temperature
            });
        }
        #endregion UPDATE_BROADCAST_RESPONSES

        #region REMOVE_BROADCAST_RESPONSES
        private static Response CreateRemoveBroadcastResponse(RemoveBroadcastType type, RemoveBroadcastData data)
        {
            return CreateBroadcastResponse(BroadcastResponseType.Remove, new RemoveBroadcastResponse
            {
                DataType = type,
                Data = data
            });
        }

        public static Response CreateRemoveRoomBroadcastResponse(Guid roomId)
        {
            return CreateRemoveBroadcastResponse(RemoveBroadcastType.Room, new RemoveRoomBroadcastData
            {
                RoomId = roomId
            });
        }

        public static Response CreateRemoveHeaterBroadcastResponse(Guid roomId, Guid heaterId)
        {
            return CreateRemoveBroadcastResponse(RemoveBroadcastType.Heater, new RemoveHeaterBroadcastData
            {
                RoomId = roomId,
                HeaterId = heaterId
            });
        }

        public static Response CreateRemoveHeatSensorBroadcastResponse(Guid roomId, Guid sensorId)
        {
            return CreateRemoveBroadcastResponse(RemoveBroadcastType.HeatSensor, new RemoveHeatSensorBroadcastData
            {
                RoomId = roomId,
                HeatSensorId = sensorId
            });
        }
        #endregion REMOVE_BROADCAST_RESPONSES

        #endregion BROADCAST_RESPONSES

        #region CLIENT_RESPONSES
        private static Response CreateClientResponse(ClientResponseType type, ClientResponseData data)
        {
            return CreateResponse(ResponseType.Client, new ClientResponseContent
            {
                DataType = type,
                Data = data
            });
        }

        #region GET_CLIENT_RESPONSES
        private static Response CreateGetClientResponse(GetClientType type, GetClientData data)
        {
            return CreateClientResponse(ClientResponseType.Get, new GetClientResponseData
            {
                DataType = type,
                Data = data
            });
        }

        public static Response CreateGetAllClientResponse(List<RoomDataContract> roomsDto)
        {
            return CreateGetClientResponse(GetClientType.All, new GetAllClientData
            {
                Rooms = roomsDto
            });
        }

        public static Response CreateGetRoomSuccessClientResponse(Guid roomId, string name, float height, float width, 
            List<HeaterDataContract> heaters, List<HeatSensorDataContract> sensors)
        {
            return CreateGetClientResponse(GetClientType.Room, new GetRoomClientData
            {
                RoomId = roomId,
                Result = new GetRoomClientDataResult
                {
                    Name = name,
                    Height = height,
                    Width = width,
                    Heaters = heaters,
                    HeatSensors = sensors
                }
            });
        }

        public static Response CreateGetRoomFailedClientResponse(Guid roomId)
        {
            return CreateGetClientResponse(GetClientType.Room, new GetRoomClientData
            {
                RoomId = roomId,
                Result = null
            });
        }

        public static Response CreateGetHeaterSuccessClientResponse(Guid roomId, Guid heaterId, float x, float y, float temperature, 
            bool isOn)
        {
            return CreateGetClientResponse(GetClientType.Heater, new GetHeaterClientData
            {
                RoomId = roomId,
                HeaterId = heaterId,
                Result = new GetHeaterClientDataResult
                {
                    X = x,
                    Y = y,
                    Temperature = temperature,
                    IsOn = isOn
                }
            });
        }

        public static Response CreateGetHeaterFailedClientResponse(Guid roomId, Guid heaterId)
        {
            return CreateGetClientResponse(GetClientType.Heater, new GetHeaterClientData
            {
                RoomId = roomId,
                HeaterId = heaterId,
                Result = null
            });
        }

        public static Response CreateGetHeatSensorSuccessClientResponse(Guid roomId, Guid sensorId, float x, float y, float temperature)
        {
            return CreateGetClientResponse(GetClientType.HeatSensor, new GetHeatSensorClientData
            {
                RoomId = roomId,
                HeatSensorId = sensorId,
                Result = new GetHeatSensorClientDataResult
                {
                    X = x,
                    Y = y,
                    Temperature = temperature
                }
            });
        }

        public static Response CreateGetHeatSensorFailedClientResponse(Guid roomId, Guid sensorId)
        {
            return CreateGetClientResponse(GetClientType.HeatSensor, new GetHeatSensorClientData
            {
                RoomId = roomId,
                HeatSensorId = sensorId,
                Result = null
            });
        }
        #endregion GET_CLIENT_RESPONSES

        #region ADD_CLIENT_RESPONSES
        private static Response CreateAddClientResponse(AddClientType type, bool success, AddClientData data)
        {
            return CreateClientResponse(ClientResponseType.Add, new AddClientResponseData
            {
                DataType = type,
                Success = success,
                Data = data
            });
        }

        public static Response CreateAddRoomClientResponse(Guid roomId, bool success)
        {
            return CreateAddClientResponse(AddClientType.Room, success, new AddRoomClientData
            {
                RoomId = roomId
            });
        }

        public static Response CreateAddHeaterClientResponse(Guid roomId, Guid heaterId, bool success)
        {
            return CreateAddClientResponse(AddClientType.Heater, success, new AddHeaterClientData
            {
                RoomId = roomId,
                HeaterId = heaterId
            });
        }

        public static Response CreateAddHeatSensorClientResponse(Guid roomId, Guid heatSensorId, bool success)
        {
            return CreateAddClientResponse(AddClientType.HeatSensor, success, new AddHeatSensorClientData
            {
                RoomId = roomId,
                HeatSensorId = heatSensorId
            });
        }
        #endregion ADD_CLIENT_RESPONSES

        #region UPDATE_CLIENT_RESPONES
        private static Response CreateUpdateClientResponse(UpdateClientType type, bool success, UpdateClientData data)
        {
            return CreateClientResponse(ClientResponseType.Update, new UpdateClientResponseData
            {
                DataType = type,
                Success = success,
                Data = data
            });
        }

        public static Response CreateUpdateHeaterClientResponse(Guid roomId, Guid heaterId, bool success)
        {
            return CreateUpdateClientResponse(UpdateClientType.Heater, success, new UpdateHeaterClientData
            {
                RoomId = roomId,
                HeaterId = heaterId
            });
        }

        public static Response CreateUpdateHeatSensorClientResponse(Guid roomId, Guid sensorId, bool success)
        {
            return CreateUpdateClientResponse(UpdateClientType.HeatSensor, success, new UpdateHeatSensorClientData
            {
                RoomId = roomId,
                HeatSensorId = sensorId
            });
        }
        #endregion UPDATE_CLIENT_RESPONES

        #region REMOVE_CLIENT_RESPONSES
        private static Response CreateRemoveClientResponse(RemoveClientType type, bool success, RemoveClientData data)
        {
            return CreateClientResponse(ClientResponseType.Remove, new RemoveClientResponseData
            {
                DataType = type,
                Success = success,
                Data = data
            });
        }

        public static Response CreateRemoveRoomClientResponse(Guid roomId)
        {
            return CreateRemoveClientResponse(AddBroadcastType.Room, new AddRoomBroadcastData
            {
                RoomId = roomId,
                Name = name,
                Width = width,
                Height = height
            });
        }
        #endregion REMOVE_CLIENT_RESPONSES

        #endregion CLIENT_RESPONSES
    }

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
        public string Name { get; set; } = "No Name";
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

    #region REQUEST
    public enum RequestType
    {
        Get = 0,
        Add = 1,
        Update = 2,
        Remove = 3
    }

    [XmlRoot("Request")]
    public class Request
    {
        public RequestType ContentType { get; set; }

        [XmlElement("GetContent", typeof(GetRequestContent))]
        [XmlElement("AddContent", typeof(AddRequestContent))]
        [XmlElement("UpdateContent", typeof(UpdateRequestContent))]
        [XmlElement("RemoveContent", typeof(RemoveRequestContent))]
        public RequestContent Content { get; set; }
    }

    [XmlInclude(typeof(GetRequestContent))]
    [XmlInclude(typeof(AddRequestContent))]
    [XmlInclude(typeof(UpdateRequestContent))]
    [XmlInclude(typeof(RemoveRequestContent))]
    public abstract class RequestContent { }


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
        public GetRequestType DataType { get; set; }

        [XmlElement("AllData", typeof(GetAllRequestData))]
        [XmlElement("RoomData", typeof(GetRoomRequestData))]
        [XmlElement("HeaterData", typeof(GetHeaterRequestData))]
        [XmlElement("HeatSensorData", typeof(GetHeatSensorRequestData))]
        public GetRequestData Data { get; set; }
    }

    [XmlInclude(typeof(GetAllRequestData))]
    [XmlInclude(typeof(GetRoomRequestData))]
    [XmlInclude(typeof(GetHeaterRequestData))]
    [XmlInclude(typeof(GetHeatSensorRequestData))]
    public abstract class GetRequestData { }

    public class GetAllRequestData : GetRequestData { }

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
        public string Name { get; set; } = "Temp Name";
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
        public RemoveRequestType DataType {  get; set; }

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

    #endregion REQUEST

    #region RESPONSE
    public enum ResponseType
    {
        Broadcast = 0,
        Client = 1
    }

    [XmlRoot("Response")]
    public class Response
    {
        public ResponseType ContentType { get; set; }

        [XmlElement("Broadcast", typeof(BroadcastResponseContent))]
        [XmlElement("Client", typeof(ClientResponseContent))]
        public ResponseContent Content { get; set; }
    }

    [XmlInclude(typeof(BroadcastResponseContent))]
    [XmlInclude(typeof(ClientResponseContent))]
    public abstract class ResponseContent { }

    #region BROADCAST_RESPONSE
    public enum BroadcastResponseType
    {
        Add = 0,
        Update = 1,
        Remove = 2
    }

    public class BroadcastResponseContent : ResponseContent
    {
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
        public AddBroadcastData Data { get; set; }
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
        public UpdateBroadcastData Data { get; set; }
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
        public RemoveBroadcastData Data { get; set; }
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

    #endregion BROADCAST_RESPONSE

    #region CLIENT_RESPONSE
    public enum ClientResponseType
    {
        Get = 0,
        Add = 1,
        Update = 2,
        Remove = 3
    }

    public class ClientResponseContent : ResponseContent
    {
        public ClientResponseType DataType { get; set; }

        public ClientResponseData Data { get; set; }
    }

    public abstract class ClientResponseData { }

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

        public GetRoomClientDataResult? Result { get; set; } = null;

        [XmlIgnore]
        public bool Success => Result != null;
    }

    public class GetRoomClientDataResult
    {
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

    public class GetHeaterClientData : GetClientData
    {
        public Guid RoomId { get; set; }

        public Guid HeaterId { get; set; }

        public GetHeaterClientDataResult? Result { get; set; } = null;

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

        public GetHeatSensorClientDataResult? Result { get; set; } = null;

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
        public AddClientType DataType { get; set; }

        public bool Success { get; set; }

        [XmlElement("RoomData", typeof(AddRoomClientData))]
        [XmlElement("HeaterData", typeof(AddHeaterClientData))]
        [XmlElement("HeatSensorData", typeof(AddHeatSensorClientData))]
        public AddClientData Data { get; set; }
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
        public RemoveClientType DataType { get; set; }

        public bool Success { get; set; }

        [XmlElement("RoomData", typeof(RemoveRoomClientData))]
        [XmlElement("HeaterData", typeof(RemoveHeaterClientData))]
        [XmlElement("HeatSensorData", typeof(RemoveHeatSensorClientData))]
        public RemoveClientData Data { get; set;}
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

    #endregion CLIENT_RESPONSE

    #endregion RESPONSE
}