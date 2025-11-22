using WebSocketSharp;
using WebSocketSharp.Server;
using System;

namespace DerailValleyWebSocket;

public class WsBehavior : WebSocketBehavior
{
    public Action<string> OnClientConnected;
    public Action<string> OnClientDisconnected;
    public Action<string, string> OnMessageReceived;

    protected override void OnOpen()
    {
        OnClientConnected?.Invoke(ID);
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        if (e.IsText)
            OnMessageReceived?.Invoke(ID, e.Data);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        OnClientDisconnected?.Invoke(ID);
    }
}
