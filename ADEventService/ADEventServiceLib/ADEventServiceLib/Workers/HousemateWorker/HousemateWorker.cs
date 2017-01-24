using System;

using GK.AppCore.Threads;
using GK.AppCore.Utility;

using ADEventService.Configuration;

namespace ADEventService.Workers
{
    // ================================================================================
    public class HousemateWorker : DefaultWorker
    {
        readonly object _lock = new object();

        readonly IADESConfig _config;
        int _housemateVisitIntervalInMSec = 60 * 1000;

        // -----------------------------------------------------------------------------
        public HousemateWorker(IADESConfig config)
        {
            _config = Verify.ArgumentNotNull(config, "config");
        }

        // -----------------------------------------------------------------------------
        public override void Start()
        {
            _housemateVisitIntervalInMSec = _config.HousemateVisitIntervalInSec * 1000;

            lock (_lock) { }
            base.Start();
        }

        // -----------------------------------------------------------------------------
        public override void Stop()
        {
            base.Stop();
            lock (_lock) { }
        }

        // -----------------------------------------------------------------------------
        protected override int ExecutePieceOfWork()
        {
            try
            {
                if (_config.EchoHousemateVisits) _config.Logger.LogINFO("Inside HousemateWorker.ExecutePieceOfWork()");
            }
            catch (Exception ex)
            {
                _config.Logger.LogERROR("Inside HousemateWorker.ExecutePieceOfWork(), Ex=[" + ex.Message + "]. ");
            }

            return _housemateVisitIntervalInMSec;
        }
    }
}
