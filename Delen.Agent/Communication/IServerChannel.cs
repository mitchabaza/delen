using Delen.Core.Communication;
using Delen.Core.Tasks;

namespace Delen.Agent.Communication
{
    
      
    
    public interface IServerChannel
    {
        Response Register(RegisterWorkerRequest request);
        Response UnRegister(UnregisterWorkerRequest request);
        Response WorkComplete(TaskExecutionResult taskResult);
        Response<TaskRequest> RequestWork();
        void SendProgress(string output);
    }
}