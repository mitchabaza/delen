using System;
using System.Diagnostics;
using Delen.Agent.Abstractions;
using Delen.Agent.Communication;
using Delen.Agent.Tasks;
using Delen.Common;
using Delen.Core.Communication;
using Delen.Core.Tasks;
using Delen.Server.Tests.Common;
using Delen.Test.Common;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Delen.Agent.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class DefaultTaskExecutorFixture : FixtureBase
    {
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<IProcessRunner> _processRunner;
        private Mock<IServerChannel> _serverChannel;
        private TaskExecutor _taskExecutor;
        private TaskExecutionOptions _options;

        public override void SetupTest()
        {
            base.SetupTest();
           
            _fileSystemMock = AutoFixture.Freeze<Mock<IFileSystem>>();
            _processRunner = AutoFixture.Freeze<Mock<IProcessRunner>>();
            _serverChannel = AutoFixture.Freeze<Mock<IServerChannel>>();
            _options= AutoFixture.Freeze<TaskExecutionOptions>();

            _serverChannel.Setup(s => s.WorkComplete(It.IsAny<TaskExecutionResult>())).Returns(new Response(false, ""));
            _taskExecutor = AutoFixture.Create<TaskExecutor>();
        }

        [Test]
        public void Execute_ShouldLogAndExit_WhenRunnerNotFoundOnFileSystem()
        {
          
            var task = AutoFixture.Create<Task>();
            var expectedTaskResult = new TaskExecutionResult(TaskExecutionResult.TaskExecutionStatus.Failed,task.WorkItemId, null);
            expectedTaskResult.AddLogEntry(TaskExecutor.RunnerNotFoundOnAgent.FormatWith(new {task.Runner}));

            CreateProcess(2000);
         
            _processRunner.Setup(p => p.Start(It.IsAny<ProcessStartInfo>(), It.IsAny<EventHandler<string>>(), It.IsAny<EventHandler<string>>()));

            _fileSystemMock.Setup(s => s.Exists(It.IsAny<string>())).Returns(false);

            _serverChannel.Setup(s => s.WorkComplete(expectedTaskResult))
                .Returns(new Response(false, ""))
                .Verifiable();

            _taskExecutor.Execute(task);

            _processRunner.Verify(p => p.Start(It.IsAny<ProcessStartInfo>(), It.IsAny<EventHandler<string>>(), It.IsAny<EventHandler<string>>()), Times.Never());
            _serverChannel.Verify();
        }

        [Test]
        public void Execute_ShouldSendFailureToServerChannel_WhenProcessStartThrows()
        {
          
            var task = AutoFixture.Create<Task>();
            var expectedTaskResult = new TaskExecutionResult(TaskExecutionResult.TaskExecutionStatus.Failed, task.WorkItemId, null);
            var exception = AutoFixture.Create< System.ComponentModel.Win32Exception >();
            expectedTaskResult.AddException(exception);

            _processRunner.Setup(p => p.Start(It.IsAny<ProcessStartInfo>(), It.IsAny<EventHandler<string>>(), It.IsAny<EventHandler<string>>())).Throws(exception);
            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(true);

            _taskExecutor.Execute(task);

            _serverChannel.Verify(sc=>sc.WorkComplete(expectedTaskResult));

        }
        [Test]
        public void Execute_ShouldSendSuccessToServerChannel_WhenProcessExecutesSuccessfully()
        {
          
            var task = AutoFixture.Create<Task>();
            var expectedTaskResult = new TaskExecutionResult(TaskExecutionResult.TaskExecutionStatus.Succeeded, task.WorkItemId, null);
            expectedTaskResult.AddLogEntry(string.Empty);

            Action<ProcessStartInfo, EventHandler<string>, EventHandler<String>> startProcess = (info, exitHandler, handler) =>
            {
                var process = new FakeProcess(50);
                process.Exited += exitHandler;
                process.Start();
            };

            _processRunner.Setup(p => p.Start(It.IsAny<ProcessStartInfo>(), It.IsAny<EventHandler<string>>(), It.IsAny<EventHandler<string>>())).Callback(startProcess);
            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(true);

            _taskExecutor.Execute(task);

            _serverChannel.Verify(sc => sc.WorkComplete(expectedTaskResult));

        }

        [Test]
        public void Execute_ShouldLaunchProcessAndWaitForExitEvent()
        {
            var task = AutoFixture.Create<Task>();
            var expectedTaskResult = new TaskExecutionResult(TaskExecutionResult.TaskExecutionStatus.Failed, task.WorkItemId, null);
            expectedTaskResult.AddLogEntry(TaskExecutor.RunnerNotFoundOnAgent.FormatWith(new {task.Runner}));

            const int millisecondsBeforeExit = 3000;
            Action<ProcessStartInfo, EventHandler<string>, EventHandler<string>> startProcess = (info, exitHandler,outputHandler) =>
            {
                var process = CreateProcess(millisecondsBeforeExit);
                process.Exited += exitHandler;
                process.Start();
            };
            _processRunner.Setup(p => p.Start(It.IsAny<ProcessStartInfo>(), It.IsAny<EventHandler<string>>(), It.IsAny<EventHandler<string>>())).Callback(startProcess);

            _fileSystemMock.Setup(s => s.Exists(It.IsAny<string>())).Returns(true);


            var ms = TimeExecution(() => _taskExecutor.Execute(task));
            _processRunner.Verify(p => p.Start(It.IsAny<ProcessStartInfo>(), It.IsAny<EventHandler<string>>(), It.IsAny<EventHandler<string>>()), Times.Once());
             ms.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(millisecondsBeforeExit-100));
        }

        private TimeSpan TimeExecution(Action action)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            action();
            stopWatch.Stop();
            return stopWatch.Elapsed;
        }

       

        private FakeProcess CreateProcess(int millisecondsBeforeExit)
        {
            return new FakeProcess(millisecondsBeforeExit);

        }
    }
}