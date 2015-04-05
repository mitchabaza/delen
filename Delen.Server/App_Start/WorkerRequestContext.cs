using System;
using System.Web;
using Delen.Common;
using Delen.Server.Views;
using log4net;

namespace Delen.Server
{
    public class WorkerRequestContext : IWorkerRequestContext
    {
        private static ILog Logger = LogManager.GetLogger(typeof(WorkerRequestContext));

        public void LogHeaders()
        {

            Logger.Info("Headers: "+HttpContext.Current.Request.Headers.ToString());
                
        }
        public Guid? Token
        {
            get

            { 
                LogHeaders();
                Guid result;
                if (Guid.TryParse(HttpContext.Current.Request.Headers[ServerConfiguration.Headers.WorkerRegistrationToken], out result))
                {
                    return result;
                }
                return null;
            }
        }
    }
}