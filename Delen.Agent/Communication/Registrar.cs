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


        public bool Register()
        {
            return _serverChannel.Register(WorkerRegistrationRequestBase.Create<RegisterWorkerRequest>())
                .Succeeded;
            
        }

        public bool UnRegister()
        {
            var command = WorkerRegistrationRequestBase.Create<UnregisterWorkerRequest>();
            return _serverChannel.UnRegister(command).Succeeded;
        }
    }
}