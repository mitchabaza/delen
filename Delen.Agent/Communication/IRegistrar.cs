using System;
using Delen.Core.Communication;

namespace Delen.Agent.Communication
{
    public interface IRegistrar
    {
        Response<Guid> Register(RegisterWorkerRequest request);
        bool UnRegister();
    }
}