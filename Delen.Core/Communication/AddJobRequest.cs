using System.Collections.Generic;
using CuttingEdge.Conditions;
using Delen.Common.Serialization;
using Delen.Core.Tasks;

namespace Delen.Core.Communication
{
    
     
    
    public class AddJobRequest:IRunnable
    {
        public string Runner { get;  set; }
        public string InitiatedBy { get; set; }
        public string Arguments { get; set; }
       
        public byte[] WorkDirectoryArchive { get;  set; }
        public string ArtifactSearchFilter { get; set; }

        public AddJobRequest()
        {
        }

       
    }
}