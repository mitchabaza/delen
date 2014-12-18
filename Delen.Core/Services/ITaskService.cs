using System.Net;
using Delen.Core.Communication;
using Delen.Core.Tasks;

namespace Delen.Core.Services
{
    public interface ITaskService
    {
         Response<TaskRequest> RequestWork(IPAddress ip);
     
        Response WorkComplete (TaskExecutionResult result);
    }
}