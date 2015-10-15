using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using AutoMapper;
using Delen.Core;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Core.Persistence;
using Delen.Core.Services;
using Delen.Core.Tasks;
using Delen.Server.Tests.Common;
using Delen.Test.Common;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Delen.Server.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]

    public class TaskServiceFixture : FixtureBase
    {
        public override void SetupTest()
        {
            base.SetupTest();
            _fakeRepository = new InMemoryRepository();
            _mockRepository = new Mock<IRepository>();
            _mappingEngine = new Mock<IMappingEngine>();
            _mockWorkerRegistry = new Mock<IWorkerRegistry>();
            _mappingEngine.Setup(m => m.Map<Task>(It.IsAny<WorkItem>()))
                .Returns<WorkItem>(wi => new Task(wi.Id, "1", "2",null));

            InstantiateTaskService();
        }

        private TaskService _taskServiceWithFakeRepository;
        private TaskService _taskServiceWithMockRepository;
        private Response<TaskRequest> _taskRequest1;
        private Response<TaskRequest> _taskRequest2;

        #region Mocks/Fakes

        private InMemoryRepository _fakeRepository;
        private Mock<IMappingEngine> _mappingEngine;
        private Mock<IRepository> _mockRepository;
        private Mock<IWorkerRegistry> _mockWorkerRegistry;

        #endregion

        private const string SlowThreadName = "FirstRequest";
        private const string FastThreadName = "SecondRequest";

        private void InstantiateTaskService()
        {
            _taskServiceWithFakeRepository = new TaskService(_fakeRepository, _mappingEngine.Object,
                _mockWorkerRegistry.Object);
            _taskServiceWithMockRepository = new TaskService(_mockRepository.Object, _mappingEngine.Object,
                _mockWorkerRegistry.Object);
        }

        private WorkItem CreatePendingWorkItem(int workItemId)
        {
            var job = AutoFixture.Create<Job>();
            job.Id.Should().NotBe(0);

            var workItem = new WorkItem(job) {Id = workItemId};
            workItem.Pending();
            return workItem;
        }

        #region RequestWork

        [Test]
        public void RequestWork_ShouldAssignWorkerToWorkItemUsingIPAddress()
        {
            var ip = Guid.NewGuid();
            var registration = AutoFixture.Create<WorkerRegistration>();

            _mockWorkerRegistry.Setup(m => m.GetRegistration(It.IsAny<Guid>())).Returns(registration);
            _mockRepository.Setup(r => r.Query<WorkItem>())
                .Returns(new List<WorkItem>() {CreatePendingWorkItem(1212)}.AsQueryable());


            _mockRepository.Setup(m => m.Put(It.Is<WorkItem>(s => s.AssignedToWorker.Id.Equals(registration.Id))))
                .Verifiable();

            _taskServiceWithMockRepository.RequestWork(new WorkerRequest(ip));

            _mockRepository.VerifyAll();
        }

        [Test(Description = "Tests that resource locking is performed such that two or more simultaneous requests do not get assigned the same WorkItem")]
        public void RequestWork_ShouldLockOtherRequests_WhenSinglePendingWorkItem()
        {
            _fakeRepository.WorkItems.Add(CreatePendingWorkItem(44));
            var registration = AutoFixture.Create<WorkerRegistration>();

            _mockWorkerRegistry.Setup(m => m.GetRegistration(It.IsAny<Guid>())).Returns(registration);

            var ipAddress = Guid.NewGuid();

            var runCount = 0;
            Action action = () =>
            {
                //purposely slow down execution of first thread, so we can simulate concurrency event with second thread
                if (runCount == 0)
                {
                    Thread.Sleep(2000);
                }

                runCount++;
            };
            _fakeRepository.PreGetAction = action;
            _fakeRepository.PreQueryAction = action;


            var slowThread = new Thread(() =>

                _taskRequest1 = _taskServiceWithFakeRepository.RequestWork(new WorkerRequest(ipAddress)))
            {
                Name = SlowThreadName
            };
            var fastThread = new Thread(() =>

                _taskRequest2 = _taskServiceWithFakeRepository.RequestWork(new WorkerRequest(ipAddress)))
            {
                Name = FastThreadName
            };
            //NOTE: the order in which threads are started is irrevelant. The OS can still decide to execute threads in any order it sees fit
            slowThread.StartAndWait();
            fastThread.StartAndWait();

            //block until both threads are done executing
            slowThread.Join();
            fastThread.Join();

            AssertOneRequestGotNoWork(_taskRequest1, _taskRequest2);
        }

        [Test(Description = "Tests that resource locking is performed such that two or more simultaneous requests to not get assigned the same WorkItem")]
        public void RequestWork_ShouldLockOtherRequests_WhenMultiplePendingWorkItems()
        {
            var ipAddress2 = Guid.NewGuid();
            var ipAddress1 = Guid.NewGuid();

            _mockWorkerRegistry.Setup(r => r.GetRegistration(ipAddress1))
                .Returns(AutoFixture.Create<WorkerRegistration>());
            _mockWorkerRegistry.Setup(r => r.GetRegistration(ipAddress2))
                .Returns(AutoFixture.Create<WorkerRegistration>());

            _fakeRepository.WorkItems.Add(CreatePendingWorkItem(66));
            _fakeRepository.WorkItems.Add(CreatePendingWorkItem(33));

            var runCount = 0;
            Action action = () =>
            {
                //purposely slow down thread1 execution, so we can simulate concurrency event with thread2
              
                if (runCount==0)
                {
                    Thread.Sleep(1000);
                }
               
                runCount++;
            };
            _fakeRepository.PreGetAction = action;
            _fakeRepository.PreQueryAction = action;


            var thread1 = new Thread(() => _taskRequest1 = _taskServiceWithFakeRepository.RequestWork(new WorkerRequest(ipAddress1)))
            {
                Name = SlowThreadName
            };
            var thread2 = new Thread(() => _taskRequest2 = _taskServiceWithFakeRepository.RequestWork(new WorkerRequest(ipAddress1)))
            {
                Name = FastThreadName
            };
            //NOTE: the order in which threads are started is irrevelant.
            //OS can still decide to execute threads in any order it sees fit
            thread1.StartAndWait();
            thread2.StartAndWait();

            //block until both threads are done executing
            thread1.Join();
            thread2.Join();

            AssertRequestsGotDifferentWorkItems(new List<Response<TaskRequest>>() {_taskRequest1, _taskRequest2});
        }

        [Test]
        public void RequestWork_ShouldSetWorkItemStatusToPendingBeforeReturning()
        {
// ReSharper disable PossibleNullReferenceException
            var registration = AutoFixture.Create<WorkerRegistration>();

            _mockWorkerRegistry.Setup(m => m.GetRegistration(It.IsAny<Guid>())).Returns(registration);

            WorkItem workItem = CreatePendingWorkItem(213);
            Assert.IsNotNull(_fakeRepository);
            Debug.Assert(_fakeRepository != null, "_fakeRepository != null");
            _fakeRepository.WorkItems.Add(workItem);
            //sanity check
            _fakeRepository.Query<WorkItem>().SingleOrDefault().Status.Should().Be(WorkItemStatus.Pending);

            _taskServiceWithFakeRepository.RequestWork(new WorkerRequest(Guid.NewGuid()));

            _fakeRepository.Query<WorkItem>().SingleOrDefault().Status.Should().Be(WorkItemStatus.InProgress);
// ReSharper enable PossibleNullReferenceException
        }


        private void AssertOneRequestGotNoWork(Response<TaskRequest> taskRequest1, Response<TaskRequest> taskRequest2)
        {
            if (taskRequest1.Payload.NoWorkAvailable)
            {
                taskRequest2.Payload.NoWorkAvailable.Should().BeFalse();

            }
            else if (taskRequest2.Payload.NoWorkAvailable)
            {
                taskRequest1.Payload.NoWorkAvailable.Should().BeFalse();
            }
            else
            {
                Assert.Fail("Only one request should be assigned any work");
            }
        }

        private void AssertRequestsGotDifferentWorkItems(IEnumerable<Response<TaskRequest>> responses)
        {
            responses.Should().NotBeEmpty();
            var uniqueValues = responses.GroupBy(n => n.Payload.Task.WorkItemId).
                Select(group =>
                    new
                    {
                        WorkItemId = group.Key,
                        Count = group.Count()
                    });

            uniqueValues.Any(u => u.Count > 1).Should().BeFalse();
        }

        #endregion

        #region WorkComplete

        [TestCase(TaskExecutionResult.TaskExecutionStatus.Succeeded, WorkItemStatus.Successful)]
        [TestCase(TaskExecutionResult.TaskExecutionStatus.Failed, WorkItemStatus.Failed)]
        public void WorkComplete_ShouldUpdateWorkItemWithCorrectStatus(
            TaskExecutionResult.TaskExecutionStatus taskStatus, WorkItemStatus workItemStatus)
        {
            var taskResult = new TaskExecutionResult(taskStatus, 1, null);

            var workItem = AutoFixture.Create<WorkItem>();
            workItem.AssignTo(AutoFixture.Create<WorkerRegistration>());
            _mockRepository.Setup(r => r.Get<WorkItem>(taskResult.WorkItemId)).Returns(workItem);
            _mockRepository.Setup(r => r.Put(It.Is<WorkItem>(wi => wi.Status.Equals(workItemStatus)))).Verifiable();

            _taskServiceWithMockRepository.WorkComplete(new WorkerRequest<TaskExecutionResult>(Guid.NewGuid()) { Body = taskResult });

            _mockRepository.VerifyAll();
        }

        [Test]
        public void WorkComplete_ShouldReturnFalse_WhenWorkItemNotInProgress()
        {
            var taskResult = AutoFixture.Create<TaskExecutionResult>();

            var workItem = AutoFixture.Create<WorkItem>();
            workItem.Pending();
            _mockRepository.Setup(r => r.Get<WorkItem>(taskResult.WorkItemId)).Returns(workItem);

            var response = _taskServiceWithMockRepository.WorkComplete(new WorkerRequest<TaskExecutionResult>(Guid.NewGuid()) { Body = taskResult });

            response.Succeeded.Should().BeFalse();
        }

        [Test]
        public void WorkComplete_ShouldReturnFalse_WhenWorkItemNotFound()
        {
            var taskResult = AutoFixture.Create<TaskExecutionResult>();

            var workItem = AutoFixture.Create<WorkItem>();
            workItem.Pending();
            _mockRepository.Setup(r => r.Get<WorkItem>(taskResult.WorkItemId)).Returns((WorkItem) null);

            var response = _taskServiceWithMockRepository.WorkComplete(new WorkerRequest<TaskExecutionResult>(Guid.NewGuid()) { Body = taskResult });

            response.Succeeded.Should().BeFalse();
        }

        #endregion
    }
}