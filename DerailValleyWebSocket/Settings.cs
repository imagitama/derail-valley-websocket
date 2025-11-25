using UnityModManagerNet;

namespace DerailValleyWebSocket;

public class Settings : UnityModManager.ModSettings
{
    public int Port = 9450;

    public override void Save(UnityModManager.ModEntry modEntry)
    {
        Save(this, modEntry);
    }
}
