using UnityModManagerNet;

namespace DerailValleyWebSocket;

public class Settings : UnityModManager.ModSettings, IDrawable
{
    [Draw(Label = "How often to emit messages (default 0.05 or 20hz)")]
    public float Rate = 0.05f; // 20hz
    [Draw(Label = "Requires restart")]
    public int Port = 9450;
    [Draw(Label = "If to emit every single var separately. May cause lag.")]
    public bool EmitEachVar = false;

    public override void Save(UnityModManager.ModEntry modEntry)
    {
        Save(this, modEntry);
    }

    public void OnChange()
    { }
}
