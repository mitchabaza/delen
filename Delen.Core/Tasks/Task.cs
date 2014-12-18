using Delen.Common.Serialization;
using Raven.Imports.Newtonsoft.Json;
using Validation;

namespace Delen.Core.Tasks
{
    

    public class Task: IRunnable
    {
        [JsonConstructor]
        public Task(int workItemId, string runner, string arguments, byte[] workDirectoryArchive)
        {
            Requires.Range(workItemId>0, "id");
            Requires.NotNullOrWhiteSpace(runner, "runner");
            WorkItemId = workItemId;
            Runner = runner;
            Arguments = arguments;
            WorkDirectoryArchive = workDirectoryArchive;
        }
       
        protected bool Equals(Task other)
        {
            return WorkItemId.Equals(other.WorkItemId);
        }

       

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Task) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Runner != null ? Runner.GetHashCode() : 0)*397) ^
                       (Arguments != null ? Arguments.GetHashCode() : 0);
            }
        }

        public int WorkItemId { get; private set; }
        public string Runner { get; private set; }
        public string Arguments { get; private set; }
        public byte[] WorkDirectoryArchive { get; private set; }
        public string ArtifactSearchFilter { get; set; }

        public override string ToString()
        {
            return string.Format("WorkItemId: {0}, Runner: {1}, Arguments: {2}", WorkItemId, Runner, Arguments);
        }
    }
}