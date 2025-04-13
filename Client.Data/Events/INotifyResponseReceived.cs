using TPUM.XmlShared.Response.Client;

namespace TPUM.Client.Data.Events
{
    internal delegate void ResponseReceivedEventHandler(object? source, ClientResponseContent response);
    internal interface INotifyResponseReceived
    {
        event ResponseReceivedEventHandler? ResponseReceived;
    }
}
