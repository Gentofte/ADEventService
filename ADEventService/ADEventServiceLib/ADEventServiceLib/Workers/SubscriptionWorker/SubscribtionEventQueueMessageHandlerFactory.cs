using Microsoft.Practices.Unity;

using GK.AppCore.Queues;
using GK.AppCore.Utility;
using GK.AppCore.Http;
using GK.AppCore.Error;

using GK.AD.MAP;

using ADEventService.Configuration;
using ADEventService.Models;

namespace ADEventService.Workers
{
    // ================================================================================
    public class SubscribtionEventQueueMessageHandlerFactory : ISubscribtionEventQueueMessageHandlerFactory
    {
        readonly IADESConfig _config;
        readonly IHttpHelperFactory _httpClientFatory;
        readonly IADEventMapper _adEventMapper;
        readonly ICircuitBreakerFactory _circuitBreakerFactory;

        // -----------------------------------------------------------------------------
        public SubscribtionEventQueueMessageHandlerFactory(IADESConfig config, IHttpHelperFactory httpClientFatory, IADEventMapper adEventMapper, ICircuitBreakerFactory circuitBreakerFactory)
        {
            _config = GK.AppCore.Utility.Verify.ArgumentNotNull(config, "config");
            _httpClientFatory = GK.AppCore.Utility.Verify.ArgumentNotNull(httpClientFatory, "httpClientFatory");
            _adEventMapper = GK.AppCore.Utility.Verify.ArgumentNotNull(adEventMapper, "adEventMapper");

            _circuitBreakerFactory = Verify.ArgumentNotNull(circuitBreakerFactory, "circuitBreakerFactory");
        }

        // -----------------------------------------------------------------------------
        public IMessageHandler CreateSubscriptionEventMessageHandler(ISubscription subscription)
        {
            return new SubscribtionEventQueueMessageHandler(_config, subscription, _adEventMapper, _httpClientFatory, _circuitBreakerFactory);
        }
    }
}
