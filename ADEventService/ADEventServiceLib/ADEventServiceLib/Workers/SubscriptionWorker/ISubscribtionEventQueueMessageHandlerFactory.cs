using GK.AppCore.Queues;
using ADEventService.Models;

namespace ADEventService.Workers
{
    // ================================================================================
    public interface ISubscribtionEventQueueMessageHandlerFactory
    {
        IMessageHandler CreateSubscriptionEventMessageHandler(ISubscription subscription);
    }
}
