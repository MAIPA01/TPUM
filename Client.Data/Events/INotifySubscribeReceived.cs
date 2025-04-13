using TPUM.XmlShared.Response.Subscribe;

namespace TPUM.Client.Data.Events
{
    internal delegate void SubscribeReceivedEventHandler(object? source, SubscribeResponseContent subscribe);
    internal interface INotifySubscribeReceived
    {
        event SubscribeReceivedEventHandler? SubscribeReceived;
    }
}
