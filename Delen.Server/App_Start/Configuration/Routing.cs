using System.Web.Mvc;
using System.Web.Routing;
using Delen.Common;

namespace Delen.Server.Configuration
{
    public class Routing : IAppConfigurable
    {
        private void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


           
            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional,
                    database = ServerConfiguration.Database.Name
                }
                );

            routes.MapRoute("Test", ServerConfiguration.WebApplication.TestModeUrlPrefix + "/{controller}/{action}/{id}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional,
                    database = ServerConfiguration.Database.TestName
                });
        }

        public void Configure()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}