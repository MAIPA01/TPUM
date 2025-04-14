using TPUM.XmlShared.Generated;

namespace TPUM.Client.Data.Events
{
    internal delegate void ResponseReceivedEventHandler(object? source, ClientResponseContent response);
    internal interface INotifyResponseReceived
    {
        event ResponseReceivedEventHandler? ResponseReceived;
    }
}