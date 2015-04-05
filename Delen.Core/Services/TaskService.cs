using System;
using System.Linq;
using System.Net;
using System.Threading;
using AutoMapper;
using Delen.Common;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Core.Persistence;
using Delen.Core.Tasks;
using log4net;
using Validation;

namespace Delen.Core.Services
{
    public class TaskService : ITaskService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (TaskService));
        private readonly object _object = new object();
        private readonly IMappingEngine _mappingEngine;
        private readonly IWorkerRegistry _registry;
        private readonly IRepository _repository;

        public TaskService(IRepository repository, IMappingEngine mappingEngine, IWorkerRegistry registry)
        {
            _repository = repository;
            _mappingEngine = mappingEngine;
            _registry = registry;
        }

        public Response WorkComplete(WorkerRequest<TaskExecutionResult> result2)
        {
            var            result = result2.Body;
            var workItem = _repository.Get<WorkItem>(result.WorkItemId);
            if (workItem == null)
            {
                return Response.Failure("WorkItemId {WorkItemId} not found".FormatWith(new {result.WorkItemId}));
            }
            if (workItem.Status.Equals(WorkItemStatus.InProgress))
            {
                workItem.Complete(Successful(result.Status),
                    result.Messages.Count>0? result.Messages.Aggregate((current, next) => current + next):"", result.Artifacts);
                _repository.Put(workItem);
                return Response.Successful();
            }
            return Response.Failure(string.Format("WorkItem must be InProgress before it can be completed {0}", workItem.ToString()));
        }



        public Response<TaskRequest> RequestWork(WorkerRequest request)
         {
            Logger.Info(string.Format("RequestWork Begin {0} {1}", DateTime.Now.Ticks,
                Thread.CurrentThread));

            lock (_object)
            {
                var registration = _registry.GetRegistration(request.Token);
                Assumes.NotNull(registration);
                WorkItem workItem =
                    _repository.Query<WorkItem>().FirstOrDefault(wi => wi.Status == (WorkItemStatus.Pending));

                Task task = null;
                if (workItem != null)
                {
                    task = _mappingEngine.Map<Task>(workItem);
                    workItem.AssignTo(registration);
                    _repository.Put(workItem);
                }
                var taskRequest = new TaskRequest {NoWorkAvailable = task == null, Task = task};
                Logger.Info("RequestWork End " + DateTime.Now.Ticks + " " + Thread.CurrentThread);

                return new Response<TaskRequest>(true, "", taskRequest);
            }
        }

        private WorkItemStatus FromTaskExecutionResult(TaskExecutionResult.TaskExecutionStatus status)
        {
            if (status.Equals(TaskExecutionResult.TaskExecutionStatus.Succeeded))
            {
                return WorkItemStatus.Successful;
            }
            return WorkItemStatus.Failed;
        }

        private bool Successful(TaskExecutionResult.TaskExecutionStatus status)
        {
            if (status.Equals(TaskExecutionResult.TaskExecutionStatus.Succeeded))
            {
                return true;
            }
            return false;
        }
    }
}