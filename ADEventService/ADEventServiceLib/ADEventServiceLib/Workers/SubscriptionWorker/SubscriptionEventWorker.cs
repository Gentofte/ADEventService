using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Logging;
using GK.AppCore.Queues;
using GK.AppCore.Utility;
using GK.AppCore.Threads;

using ADEventService.Configuration;

namespace ADEventService.Workers
{
    // ================================================================================
    public class SubscriptionEventWorker : IWorker
    {
        readonly IADESConfig _config;
        readonly IQueueWorker _queueWorker;

        // -----------------------------------------------------------------------------
        public SubscriptionEventWorker(IADESConfig config, IQueueWorker queueWorker)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _queueWorker = Verify.ArgumentNotNull(queueWorker, "queueWorker");
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
