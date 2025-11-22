using System;
using System.Collections.Generic;

namespace DerailValleyWebSocket;

public class VarDescriptor
{
    public string Name;
    public string Unit;
    public string? Namespace;  // eg "DV"
    public string TargetPath; // eg "PlayerManager.Car"
    public string Member;     // eg "GetSpeed"

    public Func<object, object>? Converter;
    public bool RefreshInstance; // if object reference might change (e.g. PlayerManager.Car)

    public VarDescriptor(string name, string unit, string? ns, string path, string member, Func<object, object>? converter = null, bool refresh = true)
    {
        Name = name;
        Unit = unit;
        Namespace = ns;
        TargetPath = path;
        Member = member;
        Converter = converter;
        RefreshInstance = refresh;
    }
}

public static class VarRegistry
{
    public static readonly Dictionary<(string VarName, string Unit), VarDescriptor> Vars =
        new()
        {
            // TODO: map to unit dynamically

            [("car_speed", "kph")] = new VarDescriptor(
                "car_speed",
                "kph",
                null,
                "PlayerManager.Car",
                "GetAbsSpeed",
                converter: (mps) => (float)mps * 3.6f
            ),

            [("car_speedometer", "kph")] = new VarDescriptor(
                "car_speedometer",
                "kph",
                "DerailValleyWebSocket",
                "CarHelper",
                "GetCarSpeedometerValueKph"
            ),
        };
}