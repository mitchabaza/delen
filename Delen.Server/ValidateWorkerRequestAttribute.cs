using System;
using System.Net;
using System.Web.Mvc;
using Delen.Common;
using Delen.Core;
using Delen.Core.Communication;

namespace Delen.Server
{
    /// <summary>
    /// Ensures that the client is running the same version as the server.  Optionally ensures that the client is already registered as a worker
    /// </summary>
    public class ValidateWorkerRequestAttribute : ActionFilterAttribute
    {
        private readonly bool _checkThatAgentIsRegistered;

        public ValidateWorkerRequestAttribute(bool checkThatAgentIsRegistered)
        {
            _checkThatAgentIsRegistered = checkThatAgentIsRegistered;
            AssemblyVersion =
                typeof (ValidateWorkerRequestAttribute).Assembly.GetName().Version.ToString();
        }

        public IWorkerRegistry WorkerRegistry { get; set; }
        private string AssemblyVersion { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_checkThatAgentIsRegistered && !IsRegisteredWorker(filterContext))
            {
                filterContext.Result = new JsonResult()
                {
                    Data =
                        new RequestError
                        {
                            Message = "Worker at {IPAddress} has not yet registered."
                                .FormatWith(
                                    new {IPAddress = filterContext.HttpContext.Request.UserIPAddress()})
                        }
                };
                return;
            }
            var clientVersion = filterContext.HttpContext.Request.Headers[ServerConfiguration.Headers.Version];
            if (clientVersion == null || !clientVersion.Equals(AssemblyVersion))
            {
                filterContext.Result = new JsonResult()
                {
                    Data =
                        "Client is using an out-of-date version. Please upgrade to {AssemblyVersion}"
                            .FormatWith(
                                new {AssemblyVersion})
                };
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        private bool IsRegisteredWorker(ControllerContext filterContext)
        {
            return WorkerRegistry.IsRegisteredWorker(filterContext.HttpContext.Request.UserIPAddress());
        }
    }
}