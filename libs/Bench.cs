using System;
using System.Diagnostics;
using Godot;

public static class Bench
{
    public static R Benchmark<R>(string description, Func<R> fn)
    {
        var watch = Stopwatch.StartNew();
        var result = fn();
        watch.Stop();
        GD.Print($"[{description}] Time: {watch.Elapsed.TotalSeconds.ToPrecisionString()}");
        return result;
    }

    public static void Benchmark(string description, Action fn)
    {
        var watch = Stopwatch.StartNew();
        fn();
        watch.Stop();
        GD.Print($"[{description}] Time: {watch.Elapsed.TotalSeconds.ToPrecisionString()}");
    }
}