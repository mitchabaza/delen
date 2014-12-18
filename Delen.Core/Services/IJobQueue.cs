using Delen.Core.Communication;

namespace Delen.Core.Services
{
    public interface IJobQueue
    {
        CreateEntityResponse<int> Queue(AddJobRequest jobRequest);
        Response Cancel(CancelJobRequest jobRequest);
    }
}