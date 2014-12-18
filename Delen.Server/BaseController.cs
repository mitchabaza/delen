using System.Text;
using System.Web.Mvc;
using Delen.Server.Json;

namespace Delen.Server
{
    public abstract class BaseController : Controller
    {

        protected override  JsonResult Json(object data, string contentType,
            Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
        protected new JsonResult Json(object data)
        {
            return Json(data, null, null, JsonRequestBehavior.AllowGet);
        }
    }
}