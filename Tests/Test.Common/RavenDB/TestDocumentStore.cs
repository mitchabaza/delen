using System;
using System.Collections.Generic;
using Raven.Client;
using Raven.Client.Document;

namespace Delen.Test.Common.RavenDB
{
    /// <summary>
    /// Tracks open sessions and ensures they are properly disposed of
    /// </summary>
    public class TestDocumentStore : DocumentStore
    {
        public override void Dispose()
        {
            base.Dispose();
            _sessions.ForEach(s=>
            {
                try
                {
                    s.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        private readonly List<IDocumentSession> _sessions = new List<IDocumentSession>();
       

        public override IDocumentSession OpenSession()
        {
            var session = base.OpenSession();
            _sessions.Add(session);
            return session;
        }
    }
}