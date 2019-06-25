using System;
using System.Threading;
using System.Threading.Tasks;


namespace TestTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello from main thread!");
            //Threadpool.QueueUserWorkItem(ThreadProc);
            Console.WriteLine("Main thread does some work, then sleeps.");
            Thread.Sleep(1000);
            Console.WriteLine("Main thread exits.");
        }
        static void ThreadProc(Object stateInfo) 
    {
        // No state object was passed to QueueUserWorkItem, so stateInfo is null.
        Console.WriteLine("Hello from the thread pool.");
    }

    }
}
