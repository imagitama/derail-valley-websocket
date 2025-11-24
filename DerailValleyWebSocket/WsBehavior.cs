using WebSocketSharp;
using WebSocketSharp.Server;
using System;

namespace DerailValleyWebSocket;

public class WsBehavior : WebSocketBehavior
{
    public Action<string>? OnClientConnected;
    public Action<string>? OnClientDisconnected;
    public Action<string, string>? OnMessageReceived;

    protected override void OnOpen()
    {
        try
        {
            OnClientConnected?.Invoke(ID);
        }
        catch (Exception ex)
        {
            Main.Logger.Logger.Log($"WsBehavior OnOpen exception: {ex}");
        }
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        if (!e.IsText)
            return;

        try
        {
            OnMessageReceived?.Invoke(ID, e.Data);
        }
        catch (Exception ex)
        {
            Main.Logger.Logger.Log($"WsBehavior OnMessage exception: {ex}");
            Sessions.CloseSession(ID, CloseStatusCode.ServerError, ex.Message);
        }
    }

    protected override void OnClose(CloseEventArgs e)
    {
        try
        {
            OnClientDisconnected?.Invoke(ID);
        }
        catch (Exception ex)
        {
            Main.Logger.Logger.Log($"WsBehavior OnClose exception: {ex}");
        }
    }

    protected override void OnError(ErrorEventArgs e)
    {
        Main.Logger.Logger.Log($"WsBehavior OnError ({ID}): {e.Message}");
    }
}
