using ADEventService.Configuration;
using ADEventService.Models;
using GK.AppCore.Queues;
using GK.AppCore.Threads;
using GK.AppCore.Utility;

namespace ADEventService.Workers
{
    // ================================================================================
    public class SubscriptionEventWorkerFactory : ISubscriptionEventWorkerFactory
    {
        readonly IADESConfig _config;
        readonly IQueueFactory _queueFactory;
        readonly ISubscribtionEventQueueMessageHandlerFactory _handlerFactory;

        // -----------------------------------------------------------------------------
        public SubscriptionEventWorkerFactory(IADESConfig config, IQueueFactory queueFactory, ISubscribtionEventQueueMessageHandlerFactory handlerFactory)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _queueFactory = Verify.ArgumentNotNull(queueFactory, "queueFactory");
            _handlerFactory = Verify.ArgumentNotNull(handlerFactory, "handlerFactory");
        }

        // -----------------------------------------------------------------------------
        public IQueueWorker CreateSubscriptionWorker(ISubscription subscription)
        {
            var queueNameSubscriberPart = "";

            queueNameSubscriberPart = subscription.Name.Trim().Truncate(16);
            queueNameSubscriberPart = queueNameSubscriberPart.ToUpperInvariant();
            queueNameSubscriberPart = queueNameSubscriberPart.Replace(" ", "_");
            queueNameSubscriberPart = queueNameSubscriberPart.TrimEnd("_".ToCharArray());
            queueNameSubscriberPart = queueNameSubscriberPart + "-" + subscription.ID;

            string queueName = string.Format("{0}-publisher-queue-s4-{1}", _config.ApplicationPrefix.ToUpperInvariant(), queueNameSubscriberPart);

            var handler = _handlerFactory.CreateSubscriptionEventMessageHandler(subscription);
            var queueWorker = _queueFactory.CreateQueueWorker(_config.FilteredEventExchange, queueName, true, handler);

            return queueWorker;
        }

    }

}
