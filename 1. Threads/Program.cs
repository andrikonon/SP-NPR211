
using System.Text;

namespace _1._Threads;

internal class Program
{

    private static volatile bool _isCancelled = false;
    
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.Unicode;
        GetSystemInfo();
        
        Console.WriteLine("привітусикі");

        Thread TYellow = new Thread(() => MyThreadYellowMethod(8));
        TYellow.Start();

        var threadId = Thread.CurrentThread.ManagedThreadId;
        Console.WriteLine($"Main thread id {threadId}");
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine($"Main Thread {threadId} working {i + 1}");
            Thread.Sleep(400);
        }

        // _isCancelled = true;
        TYellow.Join();
        Console.WriteLine("Програма завершила роботу");
    }
    
    private static void GetSystemInfo()
    {
        int coreCount = Environment.ProcessorCount;
        Console.WriteLine($"Кількість доступних процесорних ядер: {coreCount}");
    }

    private static void MyThreadYellowMethod(int n)
    {
        int threadId = Thread.CurrentThread.ManagedThreadId;
        for (int i = 0; i < n; i++)
        {
            if (_isCancelled)
            {
                Console.WriteLine("--Thread break--");
                break;
            }
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"--Thread {threadId} Yellow Working {i + 1}--");
            Console.ForegroundColor = prevColor;
            Thread.Sleep(500);
        }
    }
}
