using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Delen.Core;
using Delen.Core.Entities;
using Delen.Core.Persistence;

namespace Delen.Server.Tests.Unit
{
    public class InMemoryRepository : IRepository
    {
        public readonly IList<WorkItem> WorkItems;

        public InMemoryRepository()
        {
            WorkItems = new List<WorkItem>();
            PreQueryAction = () => { };
            PrePutAction = () => { };
            PreGetAction = () => { };
            PreDeleteAction = () => { };
        }
        /// <summary>
        /// Delegate that runs just before <seealso cref="IRepository.Delete(Entity)"/>
        /// </summary>
        public Action PreDeleteAction { get; set; }

        /// <summary>
        /// Delegate that runs just before <seealso cref="IRepository.Put{T}(T)"/>
        /// </summary>
        public Action PrePutAction { get; set; }

        /// <summary>
        /// Delegate that runs just before <seealso cref="IRepository.Query{T}()"/>
        /// </summary>
        public Action PreQueryAction { get; set; }

        /// <summary>
        /// Delegate that runs just before  <seealso cref="IRepository.Get{T}(int)"/>
        /// </summary>
        public Action PreGetAction { get; set; }


        public T Get<T>(int id) where T : class
        {
            PreGetAction();
            return (WorkItems.ToList().SingleOrDefault(t => t.Id.Equals(id))) as T;
        }

        public void Put<T>(T item) where T : class
        {
            PrePutAction();
            var entity = item as Entity;
            var existing = WorkItems.ToList().Single(t => t.Id.Equals(entity.Id));
            WorkItems.Remove(existing);
            WorkItems.Add(item as WorkItem);
        }

        public void Delete(Entity item)
        {
            var itemToDelete = WorkItems.SingleOrDefault(s => s.Id.Equals(item.Id));
            WorkItems.Remove(itemToDelete);
        }

        public IQueryable<T> Query<T>() where T : class
        {
            PreQueryAction();

            return WorkItems.AsQueryable().Cast<T>();
        }
    }
}