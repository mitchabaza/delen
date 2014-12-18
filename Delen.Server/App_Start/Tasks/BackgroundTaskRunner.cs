using System;
using System.Web.Hosting;

namespace Delen.Server.Tasks
{
    public class BackgroundTaskRunner : IRegisteredObject
    {
        private readonly object _lock = new object();
        private bool _shuttingDown;

        public BackgroundTaskRunner()
        {
            HostingEnvironment.RegisterObject(this);
        }

        public void Stop(bool immediate)
        {
            lock (_lock)
            {
                _shuttingDown = true;
            }
            HostingEnvironment.UnregisterObject(this);
        }

        public void DoWork(Action work)
        {
            lock (_lock)
            {
                if (_shuttingDown)
                {
                    return;
                }
                work();
            }
        }
    }
}