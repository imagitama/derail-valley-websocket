using System;
using System.Collections.Generic;
using DV.Customization;
using DV.Spline;

namespace DerailValleyWebSocket;

public class VarDescriptor
{
    public string Name;
    public string Unit;
    public string? Namespace;  // eg "DV"
    public string TargetPath; // eg "PlayerManager.Car"
    public string Member;     // eg "GetSpeed"
    public object[]? Args;

    public Func<object, object>? Converter;
    public bool RefreshInstance; // if object reference might change (e.g. PlayerManager.Car)

    public VarDescriptor(string name, string unit, string? ns, string path, string member, object[]? args = null, Func<object, object>? converter = null, bool refresh = true)
    {
        Name = name;
        Unit = unit;
        Namespace = ns;
        TargetPath = path;
        Member = member;
        Args = args;
        Converter = converter;
        RefreshInstance = refresh;
    }
}

public static class VarRegistry
{
    static VarRegistry()
    {
        foreach (STDSimPort port in Enum.GetValues(typeof(STDSimPort)))
        {
            string name = $"port_{port}";
            string unit = "number";

            var key = VarSystem.GetKey(name, unit);

            // if we ever assign them manually
            if (VarRegistry.Vars.ContainsKey(key))
                continue;

            VarRegistry.Vars[key] = new VarDescriptor(
                name,
                unit,
                ns: "DerailValleyWebSocket",
                path: "CarHelper",
                member: "GetCarStandardPortValue",
                args: [port]
            );
        }
    }

    public static Dictionary<(string VarName, string? Unit), VarDescriptor> Vars =
        new()
        {
            // TODO: map to unit dynamically

            [("car_speed", "kph")] = new VarDescriptor(
                name: "car_speed",
                unit: "kph",
                ns: null,
                path: "PlayerManager.Car",
                member: "GetAbsSpeed",
                converter: (mps) => (float)mps * 3.6f
            ),

            [("car_speedometer", "kph")] = new VarDescriptor(
                name: "car_speedometer",
                unit: "kph",
                ns: "DerailValleyWebSocket",
                path: "CarHelper",
                member: "GetCarSpeedometerValueKph"
            ),

            [("throttle", "position")] = new VarDescriptor(
                name: "throttle",
                unit: "position", // float 0 -> 1
                ns: "DerailValleyWebSocket",
                path: "CarHelper",
                member: "GetCarThrottleLeverPosition"
            ),

            [("train_brake", "position")] = new VarDescriptor(
                name: "train_brake",
                unit: "position", // float 0 -> 1
                ns: "DerailValleyWebSocket",
                path: "CarHelper",
                member: "GetCarTrainBrakeLeverPosition"
            ),

            [("reverser", "position")] = new VarDescriptor(
                name: "train_brake",
                unit: "position", // float 0 -> 1
                ns: "DerailValleyWebSocket",
                path: "CarHelper",
                member: "GetCarTrainReverserLeverPosition"
            ),
        };
}