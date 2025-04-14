using TPUM.XmlShared.Generated;

namespace TPUM.Server.Presentation.Events
{
    internal delegate void ClientRequestReceivedEventHandler(object? source, Guid clientId, Request request);

    internal interface INotifyOnClientRequestReceived
    {
        event ClientRequestReceivedEventHandler? ClientRequestReceived;
    }
}
