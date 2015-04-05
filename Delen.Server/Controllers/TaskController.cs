using System;
using System.Net;
using System.Web.Mvc;
using Delen.Core.Communication;
using Delen.Core.Services;
using Delen.Core.Tasks;
using Delen.Server.Views;
using log4net;
using Newtonsoft.Json;

namespace Delen.Server.Controllers
{
    public class TaskController : BaseController
    {
        private readonly ITaskService _taskService;
        private readonly IWorkerRequestContext _context;
       public TaskController(ITaskService taskService, IWorkerRequestContext context)
        {
            _taskService = taskService;
            _context = context;
        }

        [ValidateWorkerRequestAttribute(true)]
        public new ActionResult Request()
        {
           
            return Json(_taskService.RequestWork(new WorkerRequest(_context.Token)));
        }
        [ValidateWorkerRequestAttribute(true)]
        public ActionResult Complete(TaskExecutionResult executionResult)
        {
            return Json(_taskService.WorkComplete(new WorkerRequest<TaskExecutionResult>(_context.Token) { Body = executionResult }));
        }
        [ValidateWorkerRequestAttribute(true)]
        public ActionResult Progess(TaskProgress progress)
        {
            Console.WriteLine(progress.Time + " " + progress.Output);
            return Json(Core.Communication.Response.Successful());
        }
    }
}