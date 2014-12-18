using Raven.Client;
using Raven.Client.Listeners;

namespace Delen.Test.Common.RavenDB
{
    public class NoStaleQueriesListener : IDocumentQueryListener
    {
        #region Implementation of IDocumentQueryListener

        public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
        {
            queryCustomization.WaitForNonStaleResults();
        }

        #endregion
    }
}