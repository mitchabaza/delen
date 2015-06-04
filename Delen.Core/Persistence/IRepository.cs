using System;
using System.Linq;
using System.ServiceModel.Channels;
using Delen.Core.Entities;
using Raven.Client;

namespace Delen.Core.Persistence
{
    public interface IRepository
    {
        T Get<T>(int id) where T : class;
        void Put<T>(T item) where T : class;
        void Delete<T>(T item) where T:Entity;
        IQueryable<T> Query<T>() where T : class;


      //  IQueryable<T> QueryWithPreload<T>(Action<IDocumentSession> preload) where T : class;
    }
}