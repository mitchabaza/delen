using System;
using Delen.Core.Communication;
using Delen.Core.Tasks;

namespace Delen.Agent.Communication
{
    
      
    
    public interface IServerChannel
    {
        Response<Guid> Register(RegisterWorkerRequest request);
        Response UnRegister();
        Response WorkComplete(TaskExecutionResult taskResult);
        Response<TaskRequest> RequestWork();
        void SendProgress(string output);
    }
}