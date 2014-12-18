using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Delen.Agent.Abstractions;
using Delen.Agent.Communication;
using Delen.Agent.Tasks;
using Delen.Common;
using Delen.Core.Tasks;
using log4net;

namespace Delen.Agent.Tasks
{
    public class TaskExecutor : ITaskExecutor
    {
        private readonly ILogger _log;
        private readonly IServerChannel _channel;
        private readonly IProcessRunner _processRunner;
        private readonly IFileSystem _fileSystem;
        private readonly string _workingDirectory;

     //   private readonly ILog _log;//= LogManager.GetLogger(typeof(DefaultTaskExecutor));

        internal const string RunnerNotFoundOnAgent = "Runner {Runner} not found on local file system";


        public TaskExecutor(TaskExecutionOptions options, ILogger logger)
        {
            _log = logger;
            _channel = options.ServerChannel;
            _processRunner = options.ProcessRunner;
            _fileSystem = options.FileSystem;
            _workingDirectory = options.WorkingDirectory;
        }

        public void Execute(Task task)
        {
            _log.Debug(string.Format("Task Started {0} at {1}", task.WorkItemId, DateTime.Now));
            if (!_fileSystem.Exists(task.Runner))
            {
                var taskExecutionResult = new TaskExecutionResult(TaskExecutionResult.TaskExecutionStatus.Failed,
                    task.WorkItemId, null);
                taskExecutionResult.AddLogEntry(RunnerNotFoundOnAgent.FormatWith(new {task.Runner}));
                _channel.WorkComplete(taskExecutionResult);
                return;
            }

            CreateWorkingDirectory(task.WorkDirectoryArchive);
            var startInfo = new ProcessStartInfo
            {
                WorkingDirectory = WorkingDirectory,
                FileName = task.Runner,
                Arguments = task.Arguments
            };
            var resetEvent = new ManualResetEvent(false);
            //spawn process on a new thread to prevent take down of main thread
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    EventHandler<string> exitEvent = (sender, args) =>
                    {
                        //signal to main thread that our work is done
                        resetEvent.Set();
                        var zip = GetsArtifacts(task);
                        var taskExecutionResult =
                            new TaskExecutionResult(TaskExecutionResult.TaskExecutionStatus.Succeeded, task.WorkItemId,
                                zip);
                        taskExecutionResult.AddLogEntry(args);
                        _channel.WorkComplete(taskExecutionResult);
                    };
                    EventHandler<string> outputEvent = (sender, output) => _log.WriteToConsole(output);

                    _processRunner.Start(startInfo, exitEvent, outputEvent);
                }
                catch (Exception e)
                {
                    _log.Error(e);
                    resetEvent.Set();
                    var result = new TaskExecutionResult(TaskExecutionResult.TaskExecutionStatus.Failed, task.WorkItemId,null);
                    result.AddException(e);
                    _channel.WorkComplete(result);
                }
            });
            resetEvent.WaitOne();
            _log.Debug(string.Format("Task Ended {0} at {1}", task.WorkItemId, DateTime.Now));
       
        }

        private byte[] GetsArtifacts(Task task)
        {
            var files = _processRunner.CollectArtifacts(task, WorkingDirectory);
            return _fileSystem.CreateZip(files);
        }


        private string WorkingDirectory
        {
            get { return _workingDirectory; }
        }

        private void CreateWorkingDirectory(byte[] bytes)
        {
            if (bytes != null)
            {
                _fileSystem.DeleteFolder(WorkingDirectory);
                _fileSystem.ExtractZipFile(bytes, WorkingDirectory);
            }
        }
    }
}