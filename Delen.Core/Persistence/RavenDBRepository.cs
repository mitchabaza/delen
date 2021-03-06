﻿using System;
using System.Linq; 
using Delen.Core.Entities;
using Raven.Client;

namespace Delen.Core.Persistence
{
    public class RavenDBRepository : IRepository 
    {
        private readonly IDocumentStore _store;

        public RavenDBRepository(IDocumentStore store)
        {
            _store = store;
        }

        public void Delete<T>(T item) where T : Entity
        {

            using (var session = _store.OpenSession())
            {
                var entity = session.Load<T>(item.Id);
                session.Delete(entity);
                session.SaveChanges();
            }
        }

        public IQueryable<T> Query<T>() where T : class
        {
            using (var session = _store.OpenSession())
            {
                return session.Query<T>();
            }
        }

        public IQueryable<T> QueryWithPreload<T>(Action<IDocumentSession> preload) where T : class
        {
            using (var session = _store.OpenSession())
            {
                preload(session);
                return session.Query<T>();
            }
        }


        public T Get<T>(int id) where T : class
        {
            T item;
            using (var session = _store.OpenSession())
            {
                item = session.Load<T>(id);
            }
            return item;
        }

        public void Put<T>(T item) where T : class
        {
            using (var session = _store.OpenSession())
            {
                session.Store(item);
                session.SaveChanges();
            }
        }
    }
}