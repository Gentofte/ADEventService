using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Logging;
using GK.AppCore.Queues;
using GK.AppCore.Utility;

using ADEAdapterLib.Configuration;

namespace ADEAdapterLib
{
    // ================================================================================
    public class IncomingEventQueuePublisher : IIncomingEventQueuePublisher
    {
        readonly IADEAConfig _config;
        readonly IQueuePublisher _publisher;

        // -----------------------------------------------------------------------------
        public IncomingEventQueuePublisher(IADEAConfig config, IQueueFactory queueFactory)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            var factory = Verify.ArgumentNotNull(queueFactory, "queueFactory");

            _publisher = factory.CreateQueuePublisher(_config.IncomingEventExchange, _config.IncomingEventQueue, true);
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
