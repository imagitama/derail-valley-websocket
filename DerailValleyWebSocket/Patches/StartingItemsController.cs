using HarmonyLib;
using UnityModManagerNet;

namespace DerailValleyWebSocket;

[HarmonyPatch(typeof(StartingItemsController), nameof(StartingItemsController.AddStartingItems))]
internal class StartingItemsControllerPatch
{
    private static UnityModManager.ModEntry.ModLogger Logger => Main.ModEntry.Logger;
    public static void Postfix()
    {
        Logger.Log("StartingItemsController.AddStartingItems Postfix");

        PlayerManager.CarChanged += OnCarChanged;

        if (PlayerManager.Car == null)
            Logger.Log($"Initial car is null");
        else
            Logger.Log($"Initial car is '{CarHelper.GetCurrentCarName()}'");

        OnCarChanged(PlayerManager.Car);
    }

    private static void OnCarChanged(TrainCar newCar)
    {
        if (newCar == null)
        {
            Logger.Log($"Car changed => null");

            Main.server.BroadcastEvent("CAR_NAME_CHANGED", null);
            return;
        }

        var newName = CarHelper.GetCurrentCarName();

        Logger.Log($"Car changed => '{newName}'");

        Main.server.BroadcastEvent("CAR_NAME_CHANGED", newName);
    }
}