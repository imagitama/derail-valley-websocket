using UnityEngine;
using System;

using DV.HUD;
using UnityEngine;

namespace DerailValleyWebSocket;

public class UpdateDriver : MonoBehaviour
{
    void Start()
    {
        Main.Logger.Logger.Log($"UpdateDriver started");
    }

    void Update()
    {
        try
        {
            var values = VarSystem.FetchAll();

            foreach (var kv in values)
            {
                var (VarName, Unit) = kv.Key;

                // Main.Logger.Logger.Log($"Fetch varName={VarName} unit={Unit} value={kv.Value}");

                Main.Server.BroadcastVar(VarName, Unit, kv.Value);
            }
        }
        catch (Exception ex)
        {
            Main.Logger.Logger.Log($"UpdateDriver failed: {ex}");
        }
    }

    private void OnDisable()
    {
        Main.Logger.Logger.Log($"UpdateDriver disabled");
    }

    private void OnDestroy()
    {
        Main.Logger.Logger.Log($"UpdateDriver destroyed");
    }
}
