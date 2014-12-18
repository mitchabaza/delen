using Delen.Common.Serialization;

namespace Delen.Core.Tasks
{
    
    public class TaskRequest
    {
        public bool NoWorkAvailable { get; set; }
        public Task Task { get; set; }
    }
}