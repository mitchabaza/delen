using System;
using System.Linq;
using System.Threading;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Core.Tasks;
using Delen.Server.Tests.Common;
using Delen.Test.Common;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using EndPoint = Delen.Agent.Communication.EndPoint;

namespace Delen.Server.Tests.Functional
{
    public class TaskFixture : DocumentStoreFixtureBase
    {
  
        [Test]
        public void RequestWork_ShouldReturnFailure_WhenWorkerNotRegistered()
        {
            QueueJob();


            var requestWorkResponse = ServerChannel.SendRequest<RequestError>(UriFactory.Create(EndPoint.RequestWork));
            requestWorkResponse.Message.Should().Contain("not yet registered");
        }

        [Test]
        public void RequestWork_ShouldReturnWorkItemAsTask_WhenWorkIsAvailable()
        {
           var jobid= QueueJob();
           var token= RegisterAgent(AutoFixture.Create<RegisterWorkerRequest>());
            var requestWorkResponse =
                ServerChannel.SendAgentRequest<Response<TaskRequest>>(UriFactory.Create(EndPoint.RequestWork),token);
            requestWorkResponse.Payload.NoWorkAvailable.Should().BeFalse();

            WorkItem workItem;
            using (var session = DocumentStore.OpenSession())
            {
                workItem = session.Query<WorkItem>().Single(f => f.Job.Id.Equals(jobid));
            }
            requestWorkResponse.Payload.Task.Arguments.Should().Be(workItem.Arguments);
            requestWorkResponse.Payload.Task.Runner.Should().Be(workItem.Runner);
            requestWorkResponse.Payload.Task.WorkItemId.Should().Be(workItem.Id);
        }

        [Test]
        public void RequestWork_ShouldReturnAndUpdateWorkItem_WhenWorkIsAvailable()
        {
            var jobId=QueueJob();

          var token=  RegisterAgent(AutoFixture.Create<RegisterWorkerRequest>());

            var requestWorkResponse =
                ServerChannel.SendAgentRequest<Response<TaskRequest>>(UriFactory.Create(EndPoint.RequestWork), token);
            requestWorkResponse.Payload.NoWorkAvailable.Should().BeFalse();

            using (var session = DocumentStore.OpenSession())
            {
                var workItem = session.Query<WorkItem>().Single(s => s.Job.Id.Equals(jobId));
                workItem.Status.Should().Be(WorkItemStatus.InProgress);
                workItem.AssignedToWorker.Id.Should().BePositive();
            }
        }

        private int QueueJob()
        {
            var queueJobRequest = AutoFixture.Create<AddJobRequest>();
            var queueResponse =
                ServerChannel.SendRequest<CreateEntityResponse<int>>(UriFactory.Create(EndPoint.QueueJob),
                    queueJobRequest);
            queueResponse.Succeeded.Should().BeTrue();
            return queueResponse.EntityIdentifier;
        }

        private Guid RegisterAgent(RegisterWorkerRequest registerWorkerRequest)
        {
 
            var registerWorkerResponse =
                ServerChannel.SendRequest<Response<Guid>>(UriFactory.Create(EndPoint.RegisterAgent),
                    registerWorkerRequest);

            registerWorkerResponse.Succeeded.Should().BeTrue();
            return registerWorkerResponse.Payload;

        }
    }
}