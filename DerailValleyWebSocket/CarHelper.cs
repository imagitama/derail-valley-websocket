using DV.HUD;
using DV.Utils;

namespace DerailValleyWebSocket;

public static class CarHelper
{
    public static string? GetCurrentCarName()
    {
        if (PlayerManager.Car == null)
            return null;

        return PlayerManager.Car.carLivery.parentType.id;
    }

    public static float? GetCarSpeedometerValueKph()
    {
        var speed = SingletonBehaviour<HUDInterfacer>.Instance?.controlsManager?.indicatorReader?.speed?.Value;
        return speed;
    }
}