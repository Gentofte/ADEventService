using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Logging;
using GK.AppCore.Queues;
using GK.AppCore.Utility;

using ADEAdapterLib.Configuration;

namespace ADEAdapterLib.Workers
{
    // ================================================================================
    public class IncomingEventWorker : IQueueWorker
    {
        readonly IADEAConfig _config;
        readonly IQueueWorker _queueWorker;

        // -----------------------------------------------------------------------------
        public IncomingEventWorker(IADEAConfig config, IQueueFactory queueFactory, IMessageHandler messageHandler)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            var factory = Verify.ArgumentNotNull(queueFactory, "queueFactory");

            _queueWorker = factory.CreateQueueWorker(_config.IncomingEventExchange, _config.IncomingEventQueue, true, messageHandler);
        }

        // -----------------------------------------------------------------------------
        public System.Guid InstanceGUID
        {
            get { return _queueWorker.InstanceGUID; }
        }

        // -----------------------------------------------------------------------------
        public string Name
        {
            get { return _queueWorker.Name; }
            set { _queueWorker.Name = value; }
        }

        // -----------------------------------------------------------------------------
        public bool Enabled
        {
            get { return _queueWorker.Enabled; }
            set { _queueWorker.Enabled = value; }
        }

        // -----------------------------------------------------------------------------
        public void Start()
        {
            _queueWorker.Start();
        }

        // -----------------------------------------------------------------------------
        public void Stop()
        {
            _queueWorker.Stop();
        }

        // -----------------------------------------------------------------------------
        public bool IsRunning
        {
            get { return _queueWorker.IsRunning; }
        }

        // -----------------------------------------------------------------------------
        public void PrepareToStop(bool value)
        {
            _queueWorker.PrepareToStop(value);
        }

        // -----------------------------------------------------------------------------
        public bool IsRequestedToStop
        {
            get { return _queueWorker.IsRequestedToStop; }
        }

        // -----------------------------------------------------------------------------
        public bool IsReadyToStop
        {
            get { return _queueWorker.IsReadyToStop; }
            set { _queueWorker.IsReadyToStop = value; }
        }
    }
}
