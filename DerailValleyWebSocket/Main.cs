using System;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;
using UnityEngine;

namespace DerailValleyWebSocket;

#if DEBUG
[EnableReloading]
#endif
public static class Main
{
    public static UnityModManager.ModEntry ModEntry;
    public static WebsocketServer server;
    public static Settings settings;

    private static bool Load(UnityModManager.ModEntry modEntry)
    {
        ModEntry = modEntry;

        Harmony? harmony = null;
        try
        {
            settings = Settings.Load<Settings>(modEntry);

            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var go = new GameObject("DerailValleyWebSocket_UpdateDriver");
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.AddComponent<UpdateDriver>();

            // TODO: restart on port change
            server = new WebsocketServer(settings.Port);
            server.Start();

            ModEntry.Logger.Log("DerailValleyWebSocket started");
        }
        catch (Exception ex)
        {
            modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
            harmony?.UnpatchAll(modEntry.Info.Id);
            return false;
        }

        modEntry.OnUnload = Unload;
        return true;
    }

    private static void OnGUI(UnityModManager.ModEntry modEntry)
    {
        settings.Draw(modEntry);
    }

    private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
    {
        settings.Save(modEntry);
    }

    private static bool Unload(UnityModManager.ModEntry entry)
    {
        server?.Stop();
        ModEntry.Logger.Log("DerailValleyWebSocket stopped");
        return true;
    }
}
