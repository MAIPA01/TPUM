using TPUM.XmlShared.Generated;

namespace TPUM.Client.Data.Events
{
    internal delegate void SubscribeReceivedEventHandler(object? source, SubscribeResponseContent subscribe);
    internal interface INotifySubscribeReceived
    {
        event SubscribeReceivedEventHandler? SubscribeReceived;
    }
}