using UnityEngine;
using System;
using UnityModManagerNet;

namespace DerailValleyWebSocket;

public class UpdateDriver : MonoBehaviour
{
    private static UnityModManager.ModEntry.ModLogger Logger => Main.ModEntry.Logger;
    private float _elapsed;

    void Start()
    {
        Logger.Log($"UpdateDriver started");
    }

    void Update()
    {
        _elapsed += Time.deltaTime;
        if (_elapsed < Main.settings.Rate)
            return;

        _elapsed = 0f;  // reset timer

        try
        {
            var values = VarSystem.FetchAll();

            // TODO: allow to configure per var?
            if (Main.settings.EmitEachVar)
            {
                foreach (var kv in values)
                {
                    var (VarName, Unit) = kv.Key;

                    // Logger.Log($"Fetch varName={VarName} unit={Unit} value={kv.Value}");

                    Main.server.BroadcastVar(VarName, Unit, kv.Value);
                }
            }
            else
            {
                Main.server.BroadcastVars(values);
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"UpdateDriver failed: {ex}");
        }
    }

    private void OnDisable()
    {
        Logger.Log($"UpdateDriver disabled");
    }

    private void OnDestroy()
    {
        Logger.Log($"UpdateDriver destroyed");
    }
}
