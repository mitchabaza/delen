using System.Linq;
using Delen.Core;
using Delen.Core.Entities;
using Delen.Core.Persistence;

namespace Delen.Server.Tasks
{
    public class JobRunner : IJobRunner
    {
        private readonly IRepository _repository;

        public JobRunner(IRepository repository)
        {
            _repository = repository;
        }

        public void RunPendingJobs()
        {
           var jobs= _repository.Query<Job>().ToList();
            foreach (var job in jobs)
            {
                
            }

        }
    }
}