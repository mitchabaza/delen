using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Delen.Server.Json;

namespace Delen.Server.Configuration
{
    public class Serialization:IAppConfigurable
    {
        public void Configure()
        {
            //ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            //ValueProviderFactories.Factories.Add(new JsonNetValueProviderFactory());
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            ModelBinders.Binders.DefaultBinder = new JsonModelBinder();


        }
    }
}