using WebSocketSharp;
using WebSocketSharp.Server;
using System;
using UnityModManagerNet;

namespace DerailValleyWebSocket;

public class WsBehavior : WebSocketBehavior
{
    private static UnityModManager.ModEntry.ModLogger Logger => Main.ModEntry.Logger;
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
            Logger.Log($"WsBehavior OnOpen exception: {ex}");
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
            Logger.Log($"WsBehavior OnMessage exception: {ex}");
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
            Logger.Log($"WsBehavior OnClose exception: {ex}");
        }
    }

    protected override void OnError(ErrorEventArgs e)
    {
        Logger.Log($"WsBehavior OnError ({ID}): {e.Message}");
    }
}
