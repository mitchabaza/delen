using System.Collections.Generic;
using Delen.Core.Entities;

namespace Delen.Core
{
    /// <summary>
    /// 1:1  
    /// </summary>
    public class DefaultJobChunkingStrategy : IJobChunkingStrategy
    {
        public IEnumerable<WorkItem> Chunk(Job job)
        {
            var workItem = new WorkItem( job);
            return new[] { workItem };
        }
    }
}