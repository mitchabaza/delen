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
        private readonly IJobChunkingStrategyFactory _factory;

        public JobQueue(IMappingEngine workerRegistry, IDocumentStore docStore, IJobChunkingStrategyFactory factory)
        {
            _mappingEngine = workerRegistry;
            _docStore = docStore;
            _factory = factory;
        }

        public CreateEntityResponse<int> Queue(AddJobRequest jobRequest)
        {
            var job = _mappingEngine.Map<AddJobRequest, Job>(jobRequest);
            job.SetJobSplittingStrategy(_factory.Create(job));
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