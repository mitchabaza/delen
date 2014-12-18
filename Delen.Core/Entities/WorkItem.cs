using System;
using System.Collections.Generic;
using System.Linq;
using Delen.Core.Tasks;
using Raven.Imports.Newtonsoft.Json;
using Validation;

namespace Delen.Core.Entities
{
    public class WorkItem : Entity, IRunnable
    {
        private WorkItem()
        {
            Runs = new List<RunAttempt>();
        }

        public WorkItem(Job parentJob, string arguments) : this()
        {
            Runner = parentJob.Runner;
            Arguments = arguments;
            WorkDirectoryArchive = parentJob.WorkDirectoryArchive;
            ArtifactSearchFilter = parentJob.ArtifactSearchFilter;
            Job = new DenormalizedReference<Job>(parentJob);
        }

        public WorkItem(Job parentJob) : this(parentJob, parentJob.Arguments)
        {
         
        }


        public DenormalizedReference<Job> Job { get; private set; }

        public WorkItemStatus Status { get; private set; }

        public DenormalizedReference<WorkerRegistration> AssignedToWorker { get; private set; }

        public string Runner { get; private set; }

        public string Arguments { get; private set; }

        public byte[] WorkDirectoryArchive { get; private set; }
        public string ArtifactSearchFilter { get; set; }

        private List<RunAttempt> Runs { get; set; }

        [JsonIgnore]
        public byte[] Artifacts
        {
            get
            {
                if (Runs.LastOrDefault() != null)
                {
                    return Runs.Last().Artifacts;
                }
                return null;
            }
        }


        public void AssignTo(WorkerRegistration registration)
        {
            Requires.NotNull(registration, "registration");
            Runs.Add(new RunAttempt(registration));
            AssignedToWorker = registration;
            SetStatus(WorkItemStatus.InProgress);
        }

        private void SetStatus(WorkItemStatus workItemStatus)
        {
            Status = workItemStatus;
        }

        public void Cancel()
        {
            SetStatus(WorkItemStatus.Cancelled);
            CompleteExistingRun("", null);
        }

        public void Pending()
        {
            SetStatus(WorkItemStatus.Pending);
        }

        public void Complete(bool success, string messages, byte[] artifact)
        {
            SetStatus(success ? WorkItemStatus.Successful : WorkItemStatus.Failed);
            CompleteExistingRun(messages, artifact);
        }

        private void CompleteExistingRun(string messages, byte[] artifact)
        {
            RunAttempt last = Runs.LastOrDefault();
            if (last != null)
                last.Complete(messages, artifact);
        }

        public override string ToString()
        {
            return string.Format("Runner: {0}, Arguments: {1}, Job: {2}, ArtifactSearchFilter: {3}, Status: {4}, AssignedToWorker: {5}", Runner, Arguments, Job, ArtifactSearchFilter, Status, AssignedToWorker);
        }

        public class RunAttempt
        {
            public RunAttempt(WorkerRegistration registration)
            {
                Start = DateTime.Now;
                Worker = registration;
            }

            public DateTime Start { get; private set; }
            public DateTime End { get; private set; }
            public string Messages { get; private set; }
            public byte[] Artifacts { get; set; }

            public DenormalizedReference<WorkerRegistration> Worker { get; private set; }

            public void Complete(string messages, byte[] result)
            {
                End = DateTime.Now;
                Messages = messages;
                Artifacts = result;
            }
        }
    }
}