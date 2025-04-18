﻿using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    internal interface IWebSocketClient : INotifyResponseReceived, INotifySubscribeReceived, INotifyBroadcastReceived,
        INotifyClientConnected, IDisposable
    {
        Task ConnectAsync(string uri);

        Task SendAsync(string request);

        Task DisconnectAsync();
    }
}