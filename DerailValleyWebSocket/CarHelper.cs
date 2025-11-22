using DV.HUD;
using DV.Utils;
using DV.Customization;
using DV.Customization.Gadgets;
using UnityEngine;

namespace DerailValleyWebSocket;

public static class CarHelper
{
    public static string? GetCurrentCarName()
    {
        if (PlayerManager.Car == null)
            return null;

        return PlayerManager.Car.carLivery.parentType.id;
    }

    public static float? CarSpeedometerValueKphFromUi()
    {
        var speed = SingletonBehaviour<HUDInterfacer>.Instance?.controlsManager?.indicatorReader?.speed?.Value;
        return speed;
    }

    public static float? GetCarSpeedometerValueKph()
    {
        var valueFromUi = CarSpeedometerValueKphFromUi();

        if (valueFromUi != null)
            return valueFromUi;

        float? speedKph = null;

        var simController = PlayerManager.Car.SimController;
        var simulationFlow = simController.SimulationFlow;

        var tractionPortsFeeder = simController.tractionPortsFeeder;

        var portId = tractionPortsFeeder.forwardSpeedPortId;

        if (simulationFlow.TryGetPort(portId, out var port))
        {
            var speed = port.Value;
            speedKph = Mathf.Abs(speed);
        }
        else if (PlayerManager.Car != null)
        {
            speedKph = PlayerManager.Car.GetAbsSpeed() * 3.6f;
        }
            
        return speedKph;
    }
}