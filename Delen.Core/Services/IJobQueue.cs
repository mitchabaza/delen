using Delen.Core.Communication;

namespace Delen.Core.Services
{
    public interface IJobQueue
    {
        CreateEntityResponse<int> Add(AddJobRequest jobRequest);
        Response Cancel(CancelJobRequest jobRequest);
    }
}