using System.Linq;
using System.Transactions;
using AutoMapper;
using Delen.Common;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Core.Persistence;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace Delen.Core.Services
{
    public class JobQueue : IJobQueue
    {
        private readonly IMappingEngine _mappingEngine;
        private readonly IDocumentStore _docStore;
        private readonly IJobChunkingStrategyFactory _jobSplitStrategyFactory;

        public JobQueue(IMappingEngine workerRegistry, IDocumentStore docStore, IJobChunkingStrategyFactory jobSplitStrategyFactory)
        {
            _mappingEngine = workerRegistry;
            _docStore = docStore;
            _jobSplitStrategyFactory = jobSplitStrategyFactory;
        }

        public CreateEntityResponse<int> Add(AddJobRequest jobRequest)
        {
            var job = _mappingEngine.Map<AddJobRequest, Job>(jobRequest);
            job.SetJobSplittingStrategy(_jobSplitStrategyFactory.Create(job));
            var workItems = job.Split();
            using (var tx = new TransactionScope())
            {

                {
                    using (var session = _docStore.OpenSession())
                    {
                        session.Store(job);
                        foreach (var workItem in workItems)
                        {
                            session.Store(workItem);
                        }


                        session.SaveChanges();
                    }
                }
                tx.Complete();
                return new CreateEntityResponse<int>(true, "Job added with {Id}".FormatWith(new {job.Id}), job.Id);
            }
        }

        public Response Cancel(CancelJobRequest
            jobRequest)
        {
            using (var session = _docStore.OpenSession())
            {
                var workItems = session.Query<WorkItem>().Where(w => w.Job.Id.Equals(jobRequest.JobId));
                foreach (var workItem in workItems)
                {
                    workItem.Cancel();
                    session.Store(workItem);
                }

                session.SaveChanges();
            }
            return new Response(true, "Job {Id} canceled".FormatWith(new {Id = jobRequest.JobId}));
        }
    }
}