using Delen.Core.Entities;

namespace Delen.Core.Services
{
    public class JobChunkingStrategyFactory : IJobChunkingStrategyFactory
    {
        private readonly int _degreeOfParallelism;
        private readonly string _tempDirectory;

        public JobChunkingStrategyFactory(int degreeOfParallelism, string tempDirectory)
        {
            _degreeOfParallelism = degreeOfParallelism;
            _tempDirectory = tempDirectory;
        }

        public IJobChunkingStrategy Create(Job job)
        {
            if (job.Runner.ToLower().Contains("nunit"))
            {
                return new NUnitJobChunkingStrategy(_degreeOfParallelism, _tempDirectory);
            }
            return new DefaultJobChunkingStrategy();
        }
    }
}