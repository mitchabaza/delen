using System;
using System.Net;
using System.Net.Sockets;
using Delen.Agent.Communication;
using Delen.Agent.Tasks;
using Delen.Core.Communication;
using log4net;

namespace Delen.Agent
{
    public class Application : IApplication
    {
        private readonly IRegistrar _registrar;
        private readonly ITaskListener _listener;
        private readonly ILog _logger;
        private Guid _token;
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
        private static string LocalIPAddress
        {
            get
            {
                string localIP = "";
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
                return localIP;
            }
        }
        public void Start()
        {
            Response<Guid> registrationResponse;

            try
            {
               registrationResponse = _registrar.Register(new RegisterWorkerRequest() { IPAddress = LocalIPAddress, Name = Dns.GetHostName() });

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new ApplicationException("Application unable to start. Worker registration failed. Check logs for additional data", ex);
            }
            if (registrationResponse.Succeeded)
            {
                _token = registrationResponse.Payload;
                _listener.Start();
            }
            else
            {
                throw new ApplicationException("Application unable to start. Check logs for additional data");
            }
        }
    }
}