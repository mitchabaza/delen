using System;
using System.Diagnostics;
using System.Threading;

namespace Delen.TestConsoleApp
{
   public class Program
    {
        static void Main(string[] args)
        {
           
            if (args[0] == "echoAndExit")
            {
                Console.WriteLine(args.Length > 1 ? args[1] : "no echo arguments");
            }
            else if (args[0] == "echoWithRepeat")
            {

                var message = args[1];
                int delay = Convert.ToInt32(args[2]);

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(message);
                    Thread.Sleep(delay);
                }
            }
            else
            {
                Process.Start(args[1], "echoAndExit");
               
            }
 
        }
    }
}
