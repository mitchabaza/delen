using System;
using System.Net;
using System.Web.Mvc;
using Delen.Core.Communication;
using Delen.Core.Services;
using Delen.Core.Tasks;
using Newtonsoft.Json;

namespace Delen.Server.Controllers
{
    public class TaskController : BaseController
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        [ValidateWorkerRequest(true)]
        public new ActionResult Request()
        {
            return Json(_taskService.RequestWork(HttpContext.Request.UserIPAddress()));
        }
        [ValidateWorkerRequest(true)]
        public ActionResult Complete(TaskExecutionResult executionResult)
        {
            return Json(_taskService.WorkComplete(executionResult));
        }
        [ValidateWorkerRequest(true)]
        public ActionResult Progess(TaskProgress progress)
        {
            Console.WriteLine(progress.Time + " " + progress.Output);
            return Json(Core.Communication.Response.Successful());
        }
    }
}