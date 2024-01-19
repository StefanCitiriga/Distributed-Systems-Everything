using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadVsThreadpool
{
    class Program
    {
        static void Main(string[] args)
        {
            double taskSum = 0, threadSum = 0;

            // Make 100 tasks
            for (int i = 0; i < 100; i++)
            {
                ParameterizedThreadStart ts = new ParameterizedThreadStart(ThreadFunction);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Task.Run(() => ts.Invoke(sw))
                    .ContinueWith(result =>
                    {
                        // Callback
                        taskSum += sw.Elapsed.Ticks;
                    });
            }

            // Make 100 threads
            for (int i = 0; i < 100; i++)
            {
                ParameterizedThreadStart ts = new ParameterizedThreadStart(ThreadFunction);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                ts += (param) => {
                    // Callback
                    Stopwatch sw = (Stopwatch)param;
                    threadSum += sw.Elapsed.Ticks;
                };
                Thread thread = new Thread(ts);
                thread.Start(sw);
            }

            Console.WriteLine("Task ticks: {0}", taskSum / 100);
            Console.WriteLine("Thread ticks: {0}", threadSum / 100);
        }

        public static void ThreadFunction(object param)
        {
            Stopwatch sw = (Stopwatch)param;
            // Stop the stopwatch once the thread is running
            sw.Stop();
        }
    }
}
