using System.Linq.Expressions;
using Delen.Core.Entities;

namespace Delen.Core.Services
{
    public interface IJobChunkingStrategyFactory
    {
        IJobChunkingStrategy Create(Job job);
    }
}