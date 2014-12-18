using System;
using System.IO;
using System.Web.Mvc;
using Delen.Common.Serialization;
using log4net;

namespace Delen.Server.Json
{
    public class JsonModelBinder : IModelBinder
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (JsonModelBinder));
        private readonly DefaultModelBinder _defaultModelBinder = new DefaultModelBinder();

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (
                !controllerContext.HttpContext.Request.ContentType.StartsWith("application/json",
                    StringComparison.OrdinalIgnoreCase))
                return _defaultModelBinder.BindModel(controllerContext,bindingContext);
            controllerContext.HttpContext.Request.InputStream.Position = 0;
            using (var streamReader = new StreamReader(controllerContext.HttpContext.Request.InputStream))
            {
                var json = streamReader.ReadToEnd();
                Log.Info("Json " + json);
                return JsonNetHelper.Deserialize(json);
            }
        }
    }
}