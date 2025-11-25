using System;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;
using UnityEngine;

namespace DerailValleyWebSocket;

public static class Main
{
    public static UnityModManager.ModEntry Logger;
    public static WebsocketServer Server;
    public static Settings Settings;

    private static bool Load(UnityModManager.ModEntry modEntry)
    {
        Logger = modEntry;

        Harmony? harmony = null;
        try
        {
            Settings = Settings.Load<Settings>(modEntry);

            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var go = new GameObject("DerailValleyWebSocket_UpdateDriver");
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.AddComponent<UpdateDriver>();

            // TODO: restart on port change
            Server = new WebsocketServer(Settings.Port);
            Server.Start();

            Logger.Logger.Log("DerailValleyWebSocket started");
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
        GUILayout.Label("Mod Settings", UnityEngine.GUI.skin.label);

        Settings.Port = int.Parse(
            GUILayout.TextField(Settings.Port.ToString())
        );
    }

    private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
    {
        Settings.Save(modEntry);
    }

    private static bool Unload(UnityModManager.ModEntry entry)
    {
        Server?.Stop();
        Logger.Logger.Log("DerailValleyWebSocket stopped");
        return true;
    }
}
