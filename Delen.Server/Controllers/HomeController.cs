using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Delen.Common;

namespace Delen.Server.Controllers
{
    public class HomeController : Controller
    {
        public List<string> ActionNames()
        {

            var types =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                where typeof(IController).IsAssignableFrom(t) && t.Name !="HomeController"
                select t;
                 List<string> actions = new List<string>();
       
            foreach (var type in types)
            {

                if (type == null)
                {
                    return Enumerable.Empty<string>().ToList();
                }
                actions.AddRange( new ReflectedControllerDescriptor(type).GetCanonicalActions().Select(x => type.Name + "_" + x.ActionName)
                    .ToList());
                
            }
            return actions;

        }
        [HttpGet]
        public ActionResult Index()
        {
            return Json(ActionNames());
            return Json("RavenDB Database: {Database}".FormatWith(new { Database = RouteData.Values["database"] }));
        }
    }
}