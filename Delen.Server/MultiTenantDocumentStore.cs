using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Delen.Common;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Changes;
using Raven.Client.Connection;
using Raven.Client.Connection.Async;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Indexes;

namespace Delen.Server
{
    public class MultiTenantDocumentStore : IDocumentStore
    {
       private void EnsureDatabaseExists()
        {
         _store.DatabaseCommands.EnsureDatabaseExists(Database);   
        }
        private readonly IDocumentStore _store;

        public MultiTenantDocumentStore(IDocumentStore store)
        {
            _store = store;
        }

        public event EventHandler AfterDispose
        {
            add { _store.AfterDispose += value; }
            remove { _store.AfterDispose -= value; }
        }

        public IDisposable AggressivelyCache()
        {
            return _store.AggressivelyCache();
        }

        public IDisposable AggressivelyCacheFor(TimeSpan cacheDuration)
        {
            return _store.AggressivelyCacheFor(cacheDuration);
        }

        public IAsyncDatabaseCommands AsyncDatabaseCommands
        {
            get { return _store.AsyncDatabaseCommands; }
        }

        public BulkInsertOperation BulkInsert(string database = null, BulkInsertOptions options = null)
        {
            return _store.BulkInsert(database, options);
        }

        public IDatabaseChanges Changes(string database = null)
        {
            return _store.Changes(database);
        }

        public DocumentConvention Conventions
        {
            get { return _store.Conventions; }
        }

        public IDatabaseCommands DatabaseCommands
        {
            get { return _store.DatabaseCommands; }
        }

        public IDisposable DisableAggressiveCaching()
        {
            return _store.DisableAggressiveCaching();
        }

        public void Dispose()
        {
            _store.Dispose();
        }

        public void ExecuteIndex(AbstractIndexCreationTask indexCreationTask)
        {
            _store.ExecuteIndex(indexCreationTask);
        }

        public void ExecuteTransformer(AbstractTransformerCreationTask transformerCreationTask)
        {
            _store.ExecuteTransformer(transformerCreationTask);
        }

        public Etag GetLastWrittenEtag()
        {
            return _store.GetLastWrittenEtag();
        }

        public string Identifier
        {
            get { return _store.Identifier; }
            set { _store.Identifier = value; }
        }

        public IDocumentStore Initialize()
        {
            var store= _store.Initialize();
           // EnsureDatabaseExists();
            return store;
        }

        public HttpJsonRequestFactory JsonRequestFactory
        {
            get { return _store.JsonRequestFactory; }
        }

        public IAsyncDocumentSession OpenAsyncSession(string database)
        {
            return _store.OpenAsyncSession(database);
        }

        public IAsyncDocumentSession OpenAsyncSession()
        {
            return _store.OpenAsyncSession();
        }

        public IDocumentSession OpenSession(string database)
        {
            return _store.OpenSession(database);
        }

        public IDocumentSession OpenSession(OpenSessionOptions sessionOptions)
        {
            return _store.OpenSession(sessionOptions);
        }

 
        public IDisposable SetRequestsTimeoutFor(TimeSpan timeout)
        {
            return _store.SetRequestsTimeoutFor(timeout);
        }

        public NameValueCollection SharedOperationsHeaders
        {
            get { return _store.SharedOperationsHeaders; }
        }

        public string Url
        {
            get { return _store.Url; }
        }

        public bool WasDisposed
        {
            get { return _store.WasDisposed; }
        }

        public IDocumentSession OpenSession()
        {
            return _store.OpenSession(Database);
        }

        private string Database
        {
            get
            {
                var route = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
                if (route.Values["database"]!=null)
                {
                    return (string) route.Values["database"];
                }
                return ServerConfiguration.Database.Name;
            }
        }
    }
}