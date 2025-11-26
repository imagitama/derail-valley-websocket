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

        return $"{PlayerManager.Car.carType} {PlayerManager.Car.carLivery.parentType.id}";
    }

    public static float? GetCarSpeedometerValueKphFromUi()
    {
        var speed = SingletonBehaviour<HUDInterfacer>.Instance?.controlsManager?.indicatorReader?.speed?.Value;
        return speed;
    }
    
    public static float? GetCarSpeedometerValueKphFromSim()
    {
        if (PlayerManager.Car?.SimController?.SimulationFlow == null)
            return null;

        float? speedKph = null;

        var simController = PlayerManager.Car.SimController;
        var simulationFlow = simController.SimulationFlow;

        var tractionPortsFeeder = simController.tractionPortsFeeder;

        if (tractionPortsFeeder == null)
            return null;

        var portId = tractionPortsFeeder.forwardSpeedPortId;

        if (simulationFlow.TryGetPort(portId, out var port))
        {
            var speed = port.Value;
            speedKph = Mathf.Abs(speed);
        }

        return speedKph;
    }

    public static float? GetCarSpeedometerValueKph()
    {
        var valueFromUi = GetCarSpeedometerValueKphFromUi();

        if (valueFromUi != null)
            return valueFromUi;

        var valueFromSim = GetCarSpeedometerValueKphFromSim();

        if (valueFromSim != null)
            return valueFromSim;

        if (PlayerManager.Car != null)
            return PlayerManager.Car.GetAbsSpeed() * 3.6f;
            
        return null;
    }

    public static float? GetCarThrottleLeverPosition()
    {
        // TODO: fallback to ports (need to find correct one)

        // 0 to 1 at consistent increments
        var leverPosition = SingletonBehaviour<HUDInterfacer>.Instance?.baseControls?.GetValue(InteriorControlsManager.ControlType.Throttle);
        return leverPosition;
    }

    public static float? GetCarTrainBrakeLeverPosition()
    {
        // TODO: fallback to ports (need to find correct one)

        // 0 to 1 at consistent increments
        var leverPosition = SingletonBehaviour<HUDInterfacer>.Instance?.baseControls?.GetValue(InteriorControlsManager.ControlType.TrainBrake);
        return leverPosition;
    }
}