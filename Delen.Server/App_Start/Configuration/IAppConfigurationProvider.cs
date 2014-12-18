using System.Collections.Specialized;
using System.Configuration;

namespace Delen.Server.Configuration
{
    public interface IAppConfigurationProvider
    {
        NameValueCollection Settings { get; }
    }

   public class AppConfigurationProvider : IAppConfigurationProvider
    {
        public AppConfigurationProvider()
        {
            Settings = ConfigurationManager.AppSettings;
        }

        public NameValueCollection Settings { get; private set; }
    }
}