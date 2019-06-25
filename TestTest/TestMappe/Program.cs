using System;
using System.Threading.Tasks;
using System.Threading;

namespace TestMappe
{
    class Program
    {
        static bool avslutt = false;
        static Mutex MutExLock = new Mutex();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello from the main thread!");
            ThreadPool.QueueUserWorkItem(threadProcedure);
            ThreadPool.QueueUserWorkItem(getUserInput);
            while(!avslutt)
            {
                //Console.SetCursorPosition(0,100);
                MutExLock.WaitOne();
                System.Console.WriteLine("Main thread starts sleeping!");
                MutExLock.ReleaseMutex();                
                Thread.Sleep(1000);
                System.Console.WriteLine("Main thread woke up!");
                
            }
            MutExLock.WaitOne();
            System.Console.WriteLine("Main thread finished");
            MutExLock.ReleaseMutex();
        }

        private static void getUserInput(object state)
        {
            while(!avslutt)
            {
                //Console.SetCursorPosition(0,110);
                MutExLock.WaitOne();
                System.Console.Write("Skriv inn et ord");
                string inntastet = Console.ReadLine();
                if(inntastet == "avslutt")
                {
                    avslutt = true;
                }
                MutExLock.ReleaseMutex();
            }
                
        }

        private static void threadProcedure(object state)
        {
            MutExLock.WaitOne();
            System.Console.WriteLine("Hello from the background thread");
            MutExLock.ReleaseMutex();
            while(!avslutt)
            {
                MutExLock.WaitOne();
                for(int i = 0; i < 10; i++)
                {
                    //Console.SetCursorPosition(100,10);
                    System.Console.WriteLine("Iterasjon: " + i);
                    Thread.Sleep(100);
                }
                MutExLock.ReleaseMutex();
            }            
        }
    }
}
