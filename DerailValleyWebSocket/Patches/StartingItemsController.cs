using HarmonyLib;

namespace DerailValleyWebSocket; 

[HarmonyPatch(typeof(StartingItemsController), nameof(StartingItemsController.AddStartingItems))]
internal class StartingItemsControllerPatch
{
    public static void Postfix()
    {
        Main.Logger.Logger.Log("StartingItemsController.AddStartingItems Postfix");

        PlayerManager.CarChanged += OnCarChanged;

        if (PlayerManager.Car == null)
            Main.Logger.Logger.Log($"Initial car is null");
        else
            Main.Logger.Logger.Log($"Initial car is '{CarHelper.GetCurrentCarName()}'");

        OnCarChanged(PlayerManager.Car);
    }

    private static void OnCarChanged(TrainCar newCar)
    {
        if (newCar == null)
        {
            Main.Logger.Logger.Log($"Car changed => null");

            Main.Server.BroadcastEvent("CAR_NAME_CHANGED", null);
            return;
        }

        var newName = CarHelper.GetCurrentCarName();

        Main.Logger.Logger.Log($"Car changed => '{newName}'");

        Main.Server.BroadcastEvent("CAR_NAME_CHANGED", newName);
    }
}