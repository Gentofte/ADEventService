
using ADEventService.Models;
using GK.AppCore.Threads;
using GK.AppCore.Queues;

namespace ADEventService.Workers
{
    // ================================================================================
    public interface ISubscriptionEventWorkerFactory
    {
        IQueueWorker CreateSubscriptionWorker(ISubscription subscription);
    }
}
