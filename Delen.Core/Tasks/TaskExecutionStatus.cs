using System;
using System.Collections.Generic;
using System.Linq;
using Delen.Common.Serialization;
using Validation;

namespace Delen.Core.Tasks
{
    public class TaskProgress
    {
        public DateTime Time { get; set; }
        public string Output { get; set; }
    }
    public class TaskExecutionResult
    {
        private List<string> _messages;
        private List<Exception> _exceptions;

        public TaskExecutionResult(TaskExecutionStatus status, int workItemId, byte[] artifacts)
        {
            Requires.Range(workItemId > 0, "workItemId");
            Status = status;
            WorkItemId = workItemId;
            Artifacts= artifacts;
             _messages = new List<string>();
            _exceptions = new List<Exception>();
        }

        public byte[] Artifacts { get; private set; }

        protected bool Equals(TaskExecutionResult other)
        {
            return Status == other.Status && _messages.SequenceEqual(other._messages) &&
                   _exceptions.SequenceEqual(other._exceptions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TaskExecutionResult) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int) Status;
                 hashCode = (hashCode*397) ^ (_messages != null ? _messages.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_exceptions != null ? _exceptions.GetHashCode() : 0);
                return hashCode;
            }
        }
        public void AddLogEntry(string log)
        {
            _messages.Add(log);
        }

        public void AddException(Exception exception)
        {
            _exceptions.Add(exception);
        }

        public List<string> Messages
        {
            get { return _messages; }
            private set { _messages = value; }
        }

        public List<Exception> Exceptions
        {
            get { return _exceptions; }
            private set { _exceptions = value; }
        }

        public TaskExecutionStatus Status { get; private set; }
        public int WorkItemId { get; private set; }
       
        public enum TaskExecutionStatus
        {
            Failed,
            Succeeded
        }
    }
}