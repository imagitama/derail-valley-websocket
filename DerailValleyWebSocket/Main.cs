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

    private static bool Load(UnityModManager.ModEntry modEntry)
    {
        Logger = modEntry;

        Harmony? harmony = null;
        try
        {
            harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var go = new GameObject("DerailValleyWebSocket_Update");
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.AddComponent<UpdateDriver>();

            Server = new WebsocketServer(9450);
            Server.Start();
            
            PlayerManager.CarChanged += OnCarChanged;

            if (PlayerManager.Car == null)
                Logger.Logger.Log($"DerailValleyWebSocket initial car is nothing");
            else
                Logger.Logger.Log($"DerailValleyWebSocket initial car is '{CarHelper.GetCurrentCarName()}'");

            OnCarChanged(PlayerManager.Car);

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

    private static void OnCarChanged(TrainCar newCar)
    {
        if (newCar == null)
        {
            Logger.Logger.Log($"DerailValleyWebSocket car changed => null");

            Main.Server.BroadcastEvent("CAR_NAME_CHANGED", null);
            return;
        }

        var newName = CarHelper.GetCurrentCarName();

        Logger.Logger.Log($"DerailValleyWebSocket car changed => '{newName}'");

        Main.Server.BroadcastEvent("CAR_NAME_CHANGED", newName);
    }

    private static bool Unload(UnityModManager.ModEntry entry)
    {
        Server?.Stop();
        Logger.Logger.Log("DerailValleyWebSocket stopped");
        return true;
    }
}
