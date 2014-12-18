using System.Net;
using Delen.Core.Communication;
using Delen.Core.Entities;

namespace Delen.Core
{
    public interface IWorkerRegistry
    {
        Response Register(RegisterWorkerRequest registerWorkerRequest);
        Response  UnRegister(UnregisterWorkerRequest registerWorkerRequest);
        WorkerRegistration GetRegistration(IPAddress ipAddress);
        bool IsRegisteredWorker(IPAddress ipAddress);
    }
}