using System.Collections.Generic;
using Delen.Core.Entities;

namespace Delen.Core
{
    public interface IJobChunkingStrategy
    {
       IEnumerable<WorkItem> Chunk(Job job);
    }
}