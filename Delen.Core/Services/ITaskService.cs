using System;
using System.Net;
using Delen.Core.Communication;
using Delen.Core.Tasks;

namespace Delen.Core.Services
{
    public interface ITaskService
    {
       //  Response<TaskRequest> RequestWork(IPAddress ip);
        Response<TaskRequest> RequestWork(WorkerRequest request);
        Response WorkComplete(WorkerRequest<TaskExecutionResult> result);
    }
}