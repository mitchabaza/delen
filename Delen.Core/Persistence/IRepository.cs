using System.Linq;
using Delen.Core.Entities;

namespace Delen.Core.Persistence
{
    public interface IRepository
    {
        T Get<T>(int id) where T : class;
        void Put<T>(T item) where T : class;
        void Delete<T>(T item) where T:Entity;
        IQueryable<T> Query<T>() where T : class;
        
    }
}