using System;
using Delen.Common;
using StringFormat;

namespace Delen.Agent.Communication
{
    public class UriFactoryForTesting : UriFactory
    {
        public UriFactoryForTesting(Settings settings) : base(settings)
        {
        }

        protected override Uri BaseUri
        {
            get
            {
                string url = TokenStringFormat.Format("http://{Server}:{Port}/{Application}/" + ServerConfiguration.WebApplication.TestModeUrlPrefix + "/",
                    new { Settings.Server, Settings.Port, Settings.Application });
                var uri = new Uri(url, UriKind.Absolute);

                return uri;
            }
        }
    }
}