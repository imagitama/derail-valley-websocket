using UnityEngine;
using System;
using UnityModManagerNet;

namespace DerailValleyWebSocket;

public class UpdateDriver : MonoBehaviour
{
    private static UnityModManager.ModEntry.ModLogger Logger => Main.ModEntry.Logger;
    void Start()
    {
        Logger.Log($"UpdateDriver started");
    }

    void Update()
    {
        try
        {
            var values = VarSystem.FetchAll();

            foreach (var kv in values)
            {
                var (VarName, Unit) = kv.Key;

                // Logger.Log($"Fetch varName={VarName} unit={Unit} value={kv.Value}");

                // TODO: broadcast ALL vars once for performance
                Main.Server.BroadcastVar(VarName, Unit, kv.Value);
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
