using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DerailValleyWebSocket;

public class WebsocketServer
{
    private WebSocketServer _server;

    private readonly Dictionary<string, ClientSession> _clients = new();

    public WebsocketServer(int port)
    {
        _server = new WebSocketServer(port);
        _server.AddWebSocketService<WsBehavior>("/dv", () =>
        {
            var behavior = new WsBehavior();
            behavior.OnMessageReceived = HandleMessage;
            behavior.OnClientConnected = id => _clients[id] = new ClientSession(id);
            behavior.OnClientDisconnected = id => _clients.Remove(id);
            return behavior;
        });
    }

    public void Start()
    {
        _server.Start();
        Main.Logger.Logger.Log("WebSocket server started");
    }

    public void Stop()
    {
        _server.Stop();
        Main.Logger.Logger.Log("WebSocket server stopped");
    }

    private void HandleMessage(string clientId, string json)
    {
        try {
            var message = JsonConvert.DeserializeObject<Message<JObject>>(json);

            if (message == null)
                throw new Exception($"Failed to deserialize message\n{json}");

            switch (message.Type)
            {
                case MessageType.Init:
                    var initPayload = JsonConvert.DeserializeObject<InitPayload>(
                        JsonConvert.SerializeObject(message.Payload)
                    );

                    Main.Logger.Logger.Log($"Initialize message from client");

                    _clients[clientId].SubscribedVars = new HashSet<(string, string)>();

                    // TODO: Only do this if names don't match
                    Broadcast<InitPayload>(
                        MessageType.Init,
                        new InitPayload {
                            CarName = CarHelper.GetCurrentCarName()
                        }
                    );
                    break;

                case MessageType.SubscribeToVar:
                    var subscribeToVarPayload = JsonConvert.DeserializeObject<SubscribeToVarPayload>(
                        JsonConvert.SerializeObject(message.Payload)
                    );

                    Main.Logger.Logger.Log($"Client wants to subscribe to var '{subscribeToVarPayload!.Name}' ({subscribeToVarPayload!.Unit})");

                    _clients[clientId].SubscribedVars.Add((subscribeToVarPayload!.Name, subscribeToVarPayload!.Unit));

                    VarSystem.Subscribe(subscribeToVarPayload!.Name, subscribeToVarPayload!.Unit);
                    break;

                case MessageType.SubscribeToEvent:
                    var subscribeToEventPayload = JsonConvert.DeserializeObject<SubscribeToEventPayload>(
                        JsonConvert.SerializeObject(message.Payload)
                    );

                    Main.Logger.Logger.Log($"Client wants to subscribe to event '{subscribeToEventPayload!.Name}'");

                    _clients[clientId].SubscribedEvents.Add(subscribeToEventPayload!.Name);
                    break;

                default:
                    throw new Exception($"Unknown message type '{message.Type}'");
            }
        }
        catch (Exception ex)
        {
                Main.Logger.Logger.Log($"Failed to handle message: {ex}");

                Broadcast<ErrorPayload>(
                    MessageType.Error,
                    new ErrorPayload {
                        Message = ex.Message
                    }
                );
        }
    }

    public void BroadcastVar(string name, string unit, object value)
    {
        Broadcast<VarPayload>(MessageType.Var, new VarPayload { Name = name, Unit = unit, Value = value });
    }

    public void BroadcastEvent(string name, object value)
    {
        Broadcast<EventPayload>(MessageType.Event, new EventPayload { Name = name, Value = value });
    }

    public void Broadcast<TPayload>(MessageType type, TPayload payload)
    {
        string json = "";

        try {
            var message = new Message<TPayload> { Type = type, Payload = payload };

            var settings = new JsonSerializerSettings
            {
                Converters = { new StringEnumConverter() }
            };

            json = JsonConvert.SerializeObject(message, settings);

            foreach (var kv in _server.WebSocketServices["/dv"].Sessions.ActiveIDs)
            {
                _server.WebSocketServices["/dv"].Sessions.SendTo(json, kv);
            }
        }
        catch (Exception ex)
        {
            Main.Logger.Logger.Log($"Failed to broadcast: {ex} json={json}");
        }
    }
}
