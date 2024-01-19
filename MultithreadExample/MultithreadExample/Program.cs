using System;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationExample
{
    class Program
    {
        
        static void Main(string[] args)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            ParameterizedThreadStart ts = new ParameterizedThreadStart(ThreadFunction);
            
            Task t1 = Task.Run(() => ts.Invoke(new object[] { 1, token }), token); // Starting up a task and asking it to invoke the method on the new thread
            Task t2 = MyTask(2, token); // This task is async so it will run synchronously until it gets to an internal await (see the MyTask method)

            Console.ReadKey();
            source.Cancel();
            
            Task t3 = Task.Run(() => ts.Invoke(new object[] { 3, token }), token);
            // Task t3 never even runs because the token is cancelled by the point it is called, so it doesn't even bother 
            // - that's why we add token as a parameter to Task.Run, to prevent it starting if the token is already cancelled

            try
            {
                Task.WaitAll(new Task[] { t1, t2, t3 }); // Waiting for all of the tasks to finish (or throw an exception after having been cancelled)
            }
            catch(AggregateException e)
            {
                Console.WriteLine("{0} tasks were cancelled", e.InnerExceptions.Count);
            }

            Console.WriteLine("Task t1 status: {0}", t1.Status.ToString());
            Console.WriteLine("Task t2 status: {0}", t2.Status.ToString());
            Console.WriteLine("Task t3 status: {0}", t3.Status.ToString());

            Console.WriteLine("\r\n Notice that Task t3 never even runs and it's status is set to cancelled. ");

            Console.ReadKey();
        }

        private static void ThreadFunction(object param)
        {
            int id = (int)((object[])param)[0]; // Extract the integer from param array
            CancellationToken token = (CancellationToken)((object[])param)[1]; // Extract the token from param array
            while (!token.IsCancellationRequested) // If this token hasn't been cancelled, we can continue
            {
                Thread.Sleep(500);
            }
            Console.WriteLine("Thread {0} Cancelled", id);
            token.ThrowIfCancellationRequested();
        }

        private static async Task MyTask(int taskNumber, CancellationToken token)
        {
            if (!token.IsCancellationRequested) // We are manually checking the token instead of passing it as a parameter to Task.Run. It's up to us 
            {
                await Task.Run(() => // Task.Run - this is when we ACTUALLY go to another thread from the thread pool. Control is passed back to the caller here
                    {
                        while (!token.IsCancellationRequested)
                        {
                            Thread.Sleep(500);
                        }
                        Console.WriteLine("Task {0} Cancelled", (int)taskNumber);
                        token.ThrowIfCancellationRequested();
                    });
            }
            else // Manually not running the Task.Run if cancellationtoken was cancelled
            {
                token.ThrowIfCancellationRequested();
            }
        }
    }
}
