using System;
using Delen.Core.Communication;

namespace Delen.Agent.Communication
{
    public class Registrar : IRegistrar
    {
        private readonly IServerChannel _serverChannel;
        private readonly int _retryAttempts;


        public Registrar(IServerChannel serverChannel, int retryAttempts = 3)
        {
            _serverChannel = serverChannel;
            _retryAttempts = retryAttempts;
        }


        public Response<Guid> Register(RegisterWorkerRequest request)
        {
            return _serverChannel.Register(request);


        }

        public bool UnRegister()
        {
            
            return _serverChannel.UnRegister().Succeeded;
        }
    }
}