using System.Threading;

namespace Delen.Server.Tests.Unit
{
    public static class TestExtensions
    {
        /// <summary>
        /// Starts a thread and blocks until it begins execution 
        /// </summary>
        /// <param name="thread"></param>
        public static void StartAndWait(this Thread thread)
        {
            thread.Start();
              
            //wait for the thread to actually start before returning
            while (!thread.IsAlive)
            {
            }
            
        }
    }
}