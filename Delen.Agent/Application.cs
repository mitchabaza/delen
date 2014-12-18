using System;
using Delen.Agent.Communication;
using Delen.Agent.Tasks;
using log4net;

namespace Delen.Agent
{
    public class Application : IApplication
    {
        private readonly IRegistrar _registrar;
        private readonly ITaskListener _listener;
        private readonly ILog _logger;

        public Application(IRegistrar registrar, ITaskListener listener, ILog logger)
        {
            _registrar = registrar;
            _listener = listener;
            _logger = logger;
        }

        public void Stop()
        {
            _registrar.UnRegister();
            _listener.Stop();
        }

        public void Start()
        {
            bool ableToRegister;

            try
            {
                ableToRegister = _registrar.Register();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new ApplicationException("Application unable to start. Check logs for additional data", ex);
            }
            if (ableToRegister)
            {
                _listener.Start();
            }
            else
            {
                throw new ApplicationException("Application unable to start. Check logs for additional data");
            }
        }
    }
}