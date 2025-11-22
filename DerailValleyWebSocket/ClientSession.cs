using System.Collections.Generic;

namespace DerailValleyWebSocket;

public class ClientSession
{
    public string Id;
    public HashSet<(string VarName, string Unit)> SubscribedVars = new();
    public HashSet<string> SubscribedEvents = new();

    public ClientSession(string id)
    {
        Id = id;
    }
}
