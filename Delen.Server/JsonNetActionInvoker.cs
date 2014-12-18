using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Delen.Server
{
    public class JsonNetActionInvoker : ControllerActionInvoker
    {
        protected override ActionResult InvokeActionMethod(ControllerContext controllerContext,
            ActionDescriptor actionDescriptor, IDictionary<string, object> parameters)
        {
            ActionResult invokeActionMethod = base.InvokeActionMethod(controllerContext, actionDescriptor, parameters);

            if (invokeActionMethod.GetType() == typeof (JsonResult))
            {
                return new JsonNetResult(invokeActionMethod as JsonResult);
            }

            return invokeActionMethod;
        }

        private class JsonNetResult : JsonResult
        {
            public JsonNetResult()
            {
                this.ContentType = "application/json";
            }

            public JsonNetResult(JsonResult existing)
            {
                this.ContentEncoding = existing.ContentEncoding;
                this.ContentType = !string.IsNullOrWhiteSpace(existing.ContentType)
                    ? existing.ContentType
                    : "application/json";
                this.Data = existing.Data;
                this.JsonRequestBehavior = existing.JsonRequestBehavior;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                if ((this.JsonRequestBehavior == JsonRequestBehavior.DenyGet) &&
                    string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    base.ExecuteResult(context); // Delegate back to allow the default exception to be thrown
                }

                HttpResponseBase response = context.HttpContext.Response;
                response.ContentType = this.ContentType;

                if (this.ContentEncoding != null)
                {
                    response.ContentEncoding = this.ContentEncoding;
                }

                if (this.Data != null)
                {
                    // Replace with your favourite serializer.  
                    var serializer = new JsonSerializer();
                    serializer.TypeNameHandling = TypeNameHandling.Objects;
                    serializer.TypeNameAssemblyFormat =
                        System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;

                    serializer.Serialize(response.Output, this.Data);
                }
            }
        }
    }
}