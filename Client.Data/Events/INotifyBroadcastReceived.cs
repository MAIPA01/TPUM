using TPUM.XmlShared.Response.Broadcast;

namespace TPUM.Client.Data.Events
{
    internal delegate void BroadcastReceivedEventHandler(object? source, BroadcastResponseContent broadcast);
    internal interface INotifyBroadcastReceived
    {
        event BroadcastReceivedEventHandler? BroadcastReceived;
    }
}
