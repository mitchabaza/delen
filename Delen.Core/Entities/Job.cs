using System;
using System.Collections.Generic;
using System.Linq;
using CuttingEdge.Conditions;
using Delen.Common.Serialization;
using Delen.Core.Communication;
using Delen.Core.Tasks;

namespace Delen.Core.Entities
{
    
    public class Job : Entity,IRunnable
    {
        protected Job()
        {
        }

        public string ArtifactSearchFilter { get; set; }
       
        private IJobChunkingStrategy _strategy;

        public IList<DenormalizedReference<WorkItem>> WorkItems { get; private set; }

        public string Runner { get; private set; }
        public string Arguments { get; private set; }
        public byte[] WorkDirectoryArchive { get; private set; }
        public string Name { get; private set; }
        public string InitiatedBy { get; private set; }
        
        public static Job Create(string runner, string initiatedBy, string arguments, byte[] workDirectoryArchive)
        {
            Condition.Requires(runner, "runner").IsNotNullOrWhiteSpace();
            Condition.Requires(initiatedBy, "intiatedBy").IsNotNullOrWhiteSpace();

            var name = CreateName(initiatedBy, runner);

            var job = new Job { Runner = runner, InitiatedBy = initiatedBy, Name = name, Arguments = arguments, WorkDirectoryArchive = workDirectoryArchive };
            return job;
        }

        public void SetJobSplittingStrategy(IJobChunkingStrategy strategy)
        {
            _strategy = strategy;
        }


        private static string CreateName(string intiatedBy, string runner)
        {
            return DateTime.Now.ToString("s") + "_" + intiatedBy + "_" + runner;
        }

        public static Job Create(AddJobRequest request)
        {
            return Create(request.Runner, request.InitiatedBy, request.Arguments, request.WorkDirectoryArchive);
        }


        public IEnumerable<WorkItem> Split()
        {
            var workItems = new List<WorkItem>();
            foreach (var workItem in _strategy.Chunk(this))
            {
                workItem.Pending();
                workItems.Add(workItem);
            }

            WorkItems = workItems.Select(w=>new DenormalizedReference<WorkItem>(w)).ToList();
            return workItems;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, InitiatedBy: {1}, Runner: {2}, Arguments: {3}, ArtifactSearchFilter: {4}", Name, InitiatedBy, Runner, Arguments, ArtifactSearchFilter);
        }
    }
}