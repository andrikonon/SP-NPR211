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

    public static async Task<TimeSpan> TimeAsync(Task task)
    {
        Stopwatch sw = Stopwatch.StartNew();
        task.Start();
        await task; // працює не зовсім так, як очікується і не можу знайти рішення
        sw.Stop();
        return sw.Elapsed;
    } 
    
}