using ADEventSatellite.Configuration;
using GK.AppCore.Queues;
using GK.AppCore.Utility;

namespace ADEventSatellite
{
    // ================================================================================
    public class ChangeNotifyEventQueuePublisher : IChangeNotifyEventQueuePublisher
    {
        readonly IADESLConfig _config;
        readonly IQueuePublisher _publisher;

        // -----------------------------------------------------------------------------
        public ChangeNotifyEventQueuePublisher(IADESLConfig config, IQueueFactory queueFactory)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            var factory = Verify.ArgumentNotNull(queueFactory, "queueFactory");

            _publisher = factory.CreateQueuePublisher(_config.ADEventExchange, _config.ADEventQueue, true);
        }

        // -----------------------------------------------------------------------------
        public void Init()
        {
            _publisher.Init();
        }

        // -----------------------------------------------------------------------------
        public void PublishMessage(byte[] message)
        {
            _publisher.PublishMessage(message);
        }

        // -----------------------------------------------------------------------------
        public void CleanupOnExit()
        {
            _publisher.CleanupOnExit();
        }
    }
}