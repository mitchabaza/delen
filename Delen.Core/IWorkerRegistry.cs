using System;
using System.Net;
using Delen.Core.Communication;
using Delen.Core.Entities;

namespace Delen.Core
{
    public interface IWorkerRegistry
    {
        Response<Guid> Register(RegisterWorkerRequest registerWorkerRequest);
        Response UnRegister(UnregisterWorkerRequest request);
        WorkerRegistration GetRegistration(Guid token);
        bool IsRegisteredWorker(Guid ipAddress);
    }
}