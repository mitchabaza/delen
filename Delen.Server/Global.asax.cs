using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Delen.Server.Configuration;
using log4net;
using log4net.Config;

namespace Delen.Server
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Logger.Info("Request: " + System.Web.HttpContext.Current.Request.RawUrl );
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            XmlConfigurator.Configure();
       
            IEnumerable<IAppConfigurable> appConfigurables = typeof(IAppConfigurable).Assembly
                                                                            .GetExportedTypes()
                                                                            .Where(x => !x.IsAbstract &&
                                                                                        typeof(IAppConfigurable).
                                                                                            IsAssignableFrom(x))
                                                                            .Select(Activator.CreateInstance)
                                                                            .Cast<IAppConfigurable>();

            foreach (IAppConfigurable appConfigurable in appConfigurables)
                appConfigurable.Configure();


        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            Logger.Error(exception);
        }
    }
}