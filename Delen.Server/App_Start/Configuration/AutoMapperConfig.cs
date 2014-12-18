using System;
using AutoMapper;
using Delen.Core;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Core.Tasks;

namespace Delen.Server.Configuration
{
    public class AutoMapperConfig : IAppConfigurable
    {
        public void Configure()
        {
            Mapper.CreateMap<RegisterWorkerRequest, WorkerRegistration>()
                .ConstructUsing(RegisterCommandCtor);
            Mapper.CreateMap<UnregisterWorkerRequest, WorkerRegistration>()
                .ConstructUsing(RegisterCommandCtor);
            Mapper.CreateMap<AddJobRequest, Job>().ConstructUsing(r => Job.Create(((AddJobRequest)r.SourceValue)));
            Mapper.CreateMap<WorkItem, Task>().ConstructUsing(TaskCtor);

        }

        private static Func<ResolutionContext, Task> TaskCtor
        {
            get
            {
                return r =>
                {
                    var source = r.SourceValue as WorkItem; 
                    return new Task(source.Id,source.Runner, source.Runner, source.WorkDirectoryArchive){ArtifactSearchFilter = source.ArtifactSearchFilter};
                };
            }
        }
        private static Func<ResolutionContext, WorkerRegistration> RegisterCommandCtor
        {
            get
            {
                return r => WorkerRegistration.Create(((RegisterWorkerRequest) r.SourceValue).IPAddress,((RegisterWorkerRequest) r.SourceValue).Name);
            }
        }
    }
}