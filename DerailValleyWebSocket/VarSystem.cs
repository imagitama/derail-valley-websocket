using System;
using System.Collections.Generic;
using System.Reflection;

namespace DerailValleyWebSocket;

public class CompiledVar
{
    public string Name;
    public string Unit;
    public object TargetInstance;
    public Func<object> Getter;
    public bool RefreshInstance;
    public string Namespace;
    public string TargetPath;
    public string Member;
    public Func<object, object> Converter;
}

public static class VarSystem
{
    public static readonly Dictionary<(string VarName, string Unit), CompiledVar> ActiveVars = new();

    public static void Subscribe(string varName, string unit)
    {
        var key = GetKey(varName, unit);

        if (!VarRegistry.Vars.TryGetValue(key, out var varDescriptor))
            throw new Exception($"Var '{varName}' ({unit}) not in registry");

        var compiledVar = BuildCompiledVar(varDescriptor);

        ActiveVars[key] = compiledVar;
    }

    public static void Unsubscribe(string varName, string unit)
    {
        ActiveVars.Remove((varName, unit));
    }

    private static CompiledVar BuildCompiledVar(VarDescriptor desc)
    {
        object target = ResolveTargetInstance(desc.Namespace, desc.TargetPath);
        var getter = BuildGetterDelegate(target, desc.Member);

        return new CompiledVar
        {
            Name = desc.Name,
            Unit = desc.Unit,
            TargetInstance = target,
            Getter = getter,
            RefreshInstance = desc.RefreshInstance,
            Namespace = desc.Namespace,
            TargetPath = desc.TargetPath,
            Member = desc.Member,
            Converter = desc.Converter
        };
    }

    private static Type? FindType(string? ns, string name)
    {
        string fullName = ns != null
            ? $"{ns}.{name}"
            : name;

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = asm.GetType(fullName, throwOnError: false, ignoreCase: false);
            if (type != null)
                return type;
        }

        return null;
    }

    private static object ResolveTargetInstance(string? ns, string targetPath)
    {
        // eg "DV.PlayerManager.Car"
        var parts = targetPath.Split('.');

        var type = FindType(ns, parts[0]);
        if (type == null)
            throw new Exception($"Could not get type for {parts[0]} path={targetPath}");

        object current = type;

        for (int i = 1; i < parts.Length; i++)
        {
            string part = parts[i];

            var staticProp = (current as Type)?.GetProperty(part, BindingFlags.Public | BindingFlags.Static);
            if (staticProp != null)
            {
                current = staticProp.GetValue(null);
                if (current == null)
                    throw new Exception($"Could not get static prop path={targetPath} part={part}");
                continue;
            }

            var instanceProp = current.GetType().GetProperty(part);
            if (instanceProp != null)
            {
                current = instanceProp.GetValue(current);
                if (current == null)
                    throw new Exception($"Could not get instance prop path={targetPath} part={part}");
                continue;
            }

            throw new Exception($"Could not find static or instance prop path={targetPath} part={part}");
        }

        return current;
    }

    private static Func<object?> BuildGetterDelegate(object target, string member)
    {
        var type = target as Type ?? target.GetType();

        var prop = type.GetProperty(member,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if (prop != null)
            return () => prop.GetValue(target is Type ? null : target);

        var method = type.GetMethod(member,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if (method != null)
            return () => method.Invoke(target is Type ? null : target, null);

        throw new Exception($"Failed to build getter delegate target={target} member={member}");
    }

    public static object Fetch(CompiledVar cv)
    {
        if (cv.RefreshInstance)
        {
            cv.TargetInstance = ResolveTargetInstance(cv.Namespace, cv.TargetPath);
            if (cv.TargetInstance == null)
                throw new Exception($"Target instance is null path={cv.TargetPath}");

            cv.Getter = BuildGetterDelegate(cv.TargetInstance, cv.Member);
            if (cv.Getter == null)
                throw new Exception($"Getter delegate is null");
        }

        var result = cv.Getter.Invoke();

        if (cv.Converter != null)
            return cv.Converter(result);
        
        return result;
    }

    public static Dictionary<(string, string), object> FetchAll()
    {
        var result = new Dictionary<(string, string), object>();

        foreach (var kv in ActiveVars)
        {
            var cv = kv.Value;

            if (cv == null)
                throw new Exception("Value is null");

            object? val = Fetch(cv);

            if (val == null)
                throw new Exception("Fetch result is null");

            result[kv.Key] = val;
        }

        return result;
    }

    public static (string VarName, string? Unit) GetKey(string varName, string? unit)
    {
        return (varName.ToLower(), unit?.ToLower());
    }
}
