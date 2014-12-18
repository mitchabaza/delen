using System;
using System.Linq;
using Delen.Agent.Communication;
using Delen.Core;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Server.Tests.Common;
using Delen.Test.Common;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Raven.Database.Server.Responders;
using ServerChannel = Delen.Server.Tests.Common.ServerChannel;

namespace Delen.Server.Tests.Functional
{
    [TestFixture]
    public class JobFixture : DocumentStoreFixtureBase
    {
        #region Tests

        [Test]
        public void Add_ShouldQueueJob()
        {
            
            AddJob((job, command) =>
            {
                job.Should().NotBeNull();
                job.Arguments.Should().NotBeBlank();
                job.Runner.Should().Be(command.Runner);
                job.Active.Should().BeTrue();
                job.Name.Should().NotBeBlank();
                job.CreationDate.Should().HaveValue();
            }
                );
        }

        [Test]
        public void Add_ShouldQueueJobWithDefaultChunkStrategy()
        {
            AddJob((job, command) =>
            {
                using (var session = DocumentStore.OpenSession())
                {
                    var workItem = session.Query<WorkItem>().Single(w => w.Job.Id.Equals(job.Id));
                    workItem.Runner.Should().Be(job.Runner);
                    workItem.Arguments.Should().Be(job.Arguments);
                    workItem.Job.Id.Should().Be(job.Id);

                    workItem.Status.Should().Be(WorkItemStatus.Pending);
                }
            });
        }

        [Test]
        public void Add_ShouldQueueWorkItemWithPendingStatus()
        {
            AddJob((job, command) =>
            {
                using (var session = DocumentStore.OpenSession())

                {
                    var workItems = session.Query<WorkItem>().Where(w => w.Job.Id.Equals(job.Id)).ToList();

                    workItems.All(w => w.Status.Equals(WorkItemStatus.Pending)).Should().BeTrue();
                }
            });
        }

        [Test]
        public void Cancel_ShouldCancelAllWorkItems()
        {
            var command = AutoFixture.Create<AddJobRequest>();
            var addResponse = ServerChannel.SendRequest<CreateEntityResponse<int>>(UriFactory.Create(EndPoint.QueueJob),
                command);

            addResponse.Succeeded.Should().BeTrue();

            var cancelcommand = new CancelJobRequest() {JobId = addResponse.EntityIdentifier};
            var cancelResponse = ServerChannel.SendRequest<Response>(UriFactory.Create(EndPoint.CancelJob),
                cancelcommand);

            cancelResponse.Succeeded.Should().BeTrue();

            using (var session = DocumentStore.OpenSession())
            {
                var job = session.Load<Job>(addResponse.EntityIdentifier);

                job.Should().NotBeNull();
                var workItems = session.Query<WorkItem>().Where(w => w.Job.Id.Equals(job.Id)).ToList();

                workItems.All(t => t.Status.Equals(WorkItemStatus.Cancelled)).Should().BeTrue();
            }
        }

        #endregion

        private void AddJob(Action<Job, AddJobRequest> assertAction)
        {
            var command = AutoFixture.Create<AddJobRequest>();
            var response = ServerChannel.SendRequest<CreateEntityResponse<int>>(UriFactory.Create(EndPoint.QueueJob),
                command);

            response.Succeeded.Should().BeTrue();
            using (var session = DocumentStore.OpenSession())
            {
                var job = session.Load<Job>(response.EntityIdentifier);

                assertAction(job, command);
            }
        }
    }
}