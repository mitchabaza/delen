using Castle.DynamicProxy;
using Delen.Common;
using log4net;

namespace Delen.Agent
{
    public class MethodLogger : IInterceptor
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MethodLogger));

    

        public void Intercept(IInvocation invocation)
        {
          
            invocation.Proceed();
            Logger.Debug("Completed calling {Class}.{Method}".FormatWith(new { invocation.Method, Class = invocation.TargetType.ToString() }));

        }
    }
}