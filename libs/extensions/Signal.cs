using System.Collections.Generic;
using System.Linq;
using Godot;

public static class SignalExtension
{
    public static void TryConnect(this GodotObject obj, StringName signal, Callable callable)
    {
        if (!obj.IsConnected(signal, callable)) obj.Connect(signal, callable);
    }

    public static void TryDisconnect(this GodotObject obj, StringName signal, Callable callable)
    {
        if (obj.IsConnected(signal, callable)) obj.Disconnect(signal, callable);
    }

    public static void TryConnectChanged(this GodotObject obj, Callable callable) => obj.TryConnect("changed", callable);
    public static void TryDisconnectChanged(this GodotObject obj, Callable callable) => obj.TryDisconnect("changed", callable);

    public static IEnumerable<Callable> GetCallablesForTarget(this GodotObject obj, StringName signal, GodotObject target) => obj.GetSignalConnectionList(signal)
        .Where(connection => connection["callable"].As<Callable>().Target == target)
        .Select(connection => connection["callable"].As<Callable>());

    public static void DisconnectAllFromTarget(this GodotObject obj, StringName signal, GodotObject target) => GetCallablesForTarget(obj, signal, target)
        .ForEach(callable => target.Disconnect(signal, callable));
}