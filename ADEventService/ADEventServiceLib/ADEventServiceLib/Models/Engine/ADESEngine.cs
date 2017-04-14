using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Practices.Unity;

using Microsoft.Owin.Hosting;

using GK.AppCore.Threads;
using GK.AppCore.Utility;

using ADEventService.Configuration;
using ADEventService.Workers;

namespace ADEventService.Models
{
    // ================================================================================
    public class ADESEngine : IADESEngine
    {
        readonly IADESConfig _config;
        readonly IWorkerCollection _workerCollection;
        readonly IUnityContainer _container;
        readonly IRuntimeInfo _runtimeInfo;
        readonly ISubscriptionRepo _subscriptionRepo;
        readonly ISubscriptionEventWorkerFactory _subscriptionWorkerFactory;

        // -----------------------------------------------------------------------------
        public ADESEngine(IADESConfig config, IWorkerCollection workerCollection, IUnityContainer container, IRuntimeInfo runtimeInfo, ISubscriptionRepo subscriptionRepo, ISubscriptionEventWorkerFactory subscriptionWorkerFactory)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _workerCollection = Verify.ArgumentNotNull(workerCollection, "workerCollection");
            _container = Verify.ArgumentNotNull(container, "container");
            _runtimeInfo = Verify.ArgumentNotNull(runtimeInfo, "runtimeInfo");
            _subscriptionRepo = Verify.ArgumentNotNull(subscriptionRepo, "subscriptionRepo");
            _subscriptionWorkerFactory = Verify.ArgumentNotNull(subscriptionWorkerFactory, "subscriptionWorkerFactory");
        }

        // -----------------------------------------------------------------------------
        public void InitEngine()
        {
            PrimeADxSubscription();
        }

        // -----------------------------------------------------------------------------
        public void StartEngine()
        {
            if (_workerCollection.IsRunning) return;
            _StartEngine();
        }

        // -----------------------------------------------------------------------------
        void PrimeADxSubscription()
        {
            Guid ADxApplID = new Guid("18BEAC30-8FE5-437C-8D79-404692AB48D1");

            var ADxSub = _subscriptionRepo.TryGet(ADxApplID);

            if (ADxSub == null)
            {
                var subReq = _subscriptionRepo.CreateSubscriptionRequest
                    (
                    ADxApplID,
                    "ADxSAW-01",
                    "Sample ADx adapter to subscription. Disable or delete subscription if you do not running the ADx sample.",
                    string.Format("{0}/{1}", "http://localhost:8810", "api/v1/notifications"),
                    ""
                    );

                ADxSub = _subscriptionRepo.CreateSubscription(subReq);

                ADxSub.Approved = true;
                ADxSub.PublishON = true;

                _subscriptionRepo.Update(ADxSub);
            }

            Debug.Assert(ADxSub != null);

            if (ADxSub.Enabled != _config.EnableADxSampleSubscription)
            {
                ADxSub.Enabled = _config.EnableADxSampleSubscription;
                _subscriptionRepo.Update(ADxSub);
            }
        }

        // -----------------------------------------------------------------------------
        public void StopEngine()
        {
            if (_workerCollection.IsRunning == false) return;
            _StopEngine();
        }

        // -----------------------------------------------------------------------------
        public bool IsEngineON
        { get { return _workerCollection.IsRunning; } }

        // -----------------------------------------------------------------------------
        public IWorkerCollection GetWorkerCollection() { return _workerCollection; }

        // -----------------------------------------------------------------------------
        void _StartEngine()
        {
            //_workerCollection.Add(_container.Resolve<IWorker>("SatelliteHousemateWorker"));
            //_workerCollection.Add(_container.Resolve<IWorker>("ChangeNotifyWorker"));
            //_workerCollection.Add(_container.Resolve<IWorker>("SendEvent2ADEWorker"));

            _workerCollection.Add(_container.Resolve<IWorker>("HousemateWorker"));
            _workerCollection.Add(_container.Resolve<IWorker>("RawEventWorker"));

            _workerCollection.Start();

            var subscriptions = _subscriptionRepo.GetAll();

            _config.Logger.LogINFO("");
            _config.Logger.LogINFO(
                string.Format("A total of {0} subscriptions was read from persistent storage (config file=[{1}])",
                subscriptions.Count().ToString("000"),
                _config.SubscriptionsConfigFilename)
                );

            _config.Logger.LogINFO("Active subscriptions will receive AD change notifications when service is running ...");

            _config.Logger.LogINFO("");
            _config.Logger.LogINFO("-->");

            foreach (var sub in subscriptions)
            {
                _config.Logger.LogINFO(string.Format("--> {0}", sub.ToString()));

                StartSubscription(sub);
            }

            _config.Logger.LogINFO("<--");
            _config.Logger.LogINFO("");
        }

        // -----------------------------------------------------------------------------
        void _StopEngine()
        {
            _workerCollection.Clear();
        }

        // -----------------------------------------------------------------------------
        void StartSubscription(ISubscription subscription)
        {
            if (subscription.Approved)
            {
                if (subscription.Enabled)
                {
                    var subscriptionWorker = _subscriptionWorkerFactory.CreateSubscriptionWorker(subscription);
                    _workerCollection.Add(subscriptionWorker);
                    subscriptionWorker.Start();
                }
            }
        }

    }
}
