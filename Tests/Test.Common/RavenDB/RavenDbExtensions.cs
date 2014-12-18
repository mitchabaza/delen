using System;
using System.Linq;
using System.Threading;
using Delen.Common;
using Delen.Core.Entities;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Document;

namespace Delen.Test.Common.RavenDB
{
    public static class RavenDbExtensions
    {
        //public static void StoreAndSaveChange(this IDocumentStore store)
        //{
        //    documentSession.SaveChanges();
        //    var staleResults = true;

        //    while (staleResults)
        //    {
        //        using (var session = store.OpenSession())
        //        {
        //            staleResults = session.Query<WorkerRegistration>()
        //                .Customize((s) => s.WaitForNonStaleResultsAsOfNow())
        //                .FirstOrDefault(f => f.IPAddress.Equals(registration.IPAddress)) != null;
        //        }
        //        Thread.Sleep(100);
        //    }
        //}
        /// <summary>
        /// <b>Permanently</b> deletes all documents of the given Type from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        public static void Clear<T>(this IDocumentSession session)
        {
            var collectionName = typeof (T).Name + "s";

            session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName",
                new IndexQuery {Query = "Tag:" + collectionName}, false
                );
        }

        /// <summary>
        /// Stores an entity to DocumentStore and waits for RavenDB to update stale indexes before returning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documentStore"></param>
        /// <param name="entity"></param>
        /// <param name="nonStaleResultsComparator"></param>
        public static void Save<T>(this IDocumentStore documentStore, T entity, Func<T,bool> nonStaleResultsComparator ) where T : Entity
        {
            using (var session = documentStore.OpenSession())
            {
                session.Store(entity);
                session.SaveChanges();
            }
            var staleResults = true;

            TimeSpan runningForLessThan = DateTime.Now.TimeOfDay;
            while (staleResults || runningForLessThan < TimeSpan.FromSeconds(5))
            {
                Thread.Sleep(100);
                Console.WriteLine(@"Sleeping while waiting for non-state results on Entity {0}", entity.ReportAllProperties());
                using (var session = documentStore.OpenSession())
                {
                    var persistedEntity = session.Query<T>()
                         .FirstOrDefault(f => nonStaleResultsComparator(f));
                    staleResults = persistedEntity == null;
                }
                    
            }
            if (staleResults)
            {
                throw new InvalidTestException("RavenDB is still returning stale results");
            }
        }
        public static void Clear(this IDocumentStore store, Type type)
        {
            var collectionName = (type.Name + "s");
            using (var session = store.OpenSession())
            {
                //var methodInfo = typeof(RavenDbExtensions).GetMethod("Clear", System.Reflection.BindingFlags.Static | BindingFlags.Public);
                //var genericArguments = new Type[] { type};
                //var genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
                //var result = genericMethodInfo.Invoke(null, null);
                session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName",
                    new IndexQuery {Query = "Tag:" + collectionName}, false
                    );
            }
        }

        public static void DeleteAllDocuments(this IDocumentStore store)
        {
            foreach (var type  in typeof (Entity).Assembly.ExportedTypes.Where(
                type =>
                    type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof (Entity))))


            {
                store.Clear(type);
            }
            store.DisableAggressiveCaching();
            store.OpenSession().ClearStaleIndexes();
        }

        public static void DeleteDatabase(this IDocumentStore store, string name, bool hardDelete = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var databaseCommands = store.DatabaseCommands;
            var relativeUrl = "/admin/databases/" + name;

            if (hardDelete)
                relativeUrl += "?hard-delete=true";

            var serverClient = databaseCommands.ForSystemDatabase() as ServerClient;
            if (serverClient == null)
                throw new ApplicationException("Please use a more intelligent exception here");

            var httpJsonRequest = serverClient.CreateRequest("DELETE", relativeUrl);
            httpJsonRequest.ExecuteRequest();
        }

        public static void ClearStaleIndexes(this IDocumentSession db)
        {
            while (db.Advanced.DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length != 0)
            {
                Thread.Sleep(10);
            }
        }
    }
}