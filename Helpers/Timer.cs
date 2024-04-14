using System.Diagnostics;

namespace Helpers;

public static class Timer
{
    public static TimeSpan Time(Action action)
    {
        Stopwatch sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        return sw.Elapsed;
    } 
}