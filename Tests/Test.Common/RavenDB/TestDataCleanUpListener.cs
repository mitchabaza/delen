using System.Collections.Generic;
using Raven.Client;
using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace Delen.Test.Common.RavenDB
{
    public class TestDataCleanUpListener : IDocumentStoreListener
    {
        private readonly List<object> _keys = new List<object>();

        public bool BeforeStore(string key, object entityInstance, RavenJObject metadata, RavenJObject original)
        {
            return true;
        }

        public void AfterStore(string key, object entityInstance, RavenJObject metadata)
        {
            _keys.Add(key);
        }

        public void CleanUp(IDocumentSession session)
        {
            _keys.ForEach(k =>
            {
                session.Advanced.DocumentStore.DatabaseCommands.Delete(k.ToString(), null);
                _keys.Remove(k);
            });

        }
    }
}