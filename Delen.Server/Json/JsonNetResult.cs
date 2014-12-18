using System;
using System.Web;
using System.Web.Mvc;
using Delen.Common.Serialization;

namespace Delen.Server.Json
{
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
          
        }

     
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            if (ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            if (Data == null)
                return;

           
            var json = JsonNetHelper.Serialize(this.Data);
            response.Write(json);
        }
    }
}