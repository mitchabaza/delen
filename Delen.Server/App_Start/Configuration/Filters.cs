using System.Web.Mvc;
using Delen.Server.Controllers;

namespace Delen.Server.Configuration
{
    public class Filters : IAppConfigurable
    {
        private void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new JsonRequestBehaviorAttribute());
           
        }

        public void Configure()
        {
            RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}