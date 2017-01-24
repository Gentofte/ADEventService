using ADEventSatellite.Configuration;
using GK.AppCore.Queues;
using GK.AppCore.Threads;
using GK.AppCore.Utility;
using Microsoft.Practices.Unity;

namespace ADEventSatellite.Model
{
    // ================================================================================
    public class Engine : IEngine
    {
        readonly IADESLConfig _config;
        readonly IWorkerCollection _workerCollection;
        readonly IUnityContainer _container;
        readonly IRuntimeInfo _runtimeInfo;

        readonly IMQHelper _mqHelper;

        // -----------------------------------------------------------------------------
        public Engine(IADESLConfig config, IWorkerCollection workerCollection, IUnityContainer container, IRuntimeInfo runtimeInfo, IMQHelper mqHelper)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _workerCollection = Verify.ArgumentNotNull(workerCollection, "workerCollection");
            _container = Verify.ArgumentNotNull(container, "container");
            _runtimeInfo = Verify.ArgumentNotNull(runtimeInfo, "runtimeInfo");

            _mqHelper = Verify.ArgumentNotNull(mqHelper, "mqHelper");
        }

        // -----------------------------------------------------------------------------
        public void InitEngine()
        {
        }

        // -----------------------------------------------------------------------------
        public void StartEngine()
        {
            if (_workerCollection.IsRunning) return;
            _StartEngine();
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
        public void Reset()
        {
            var isRunning = _workerCollection.IsRunning;

            _StopEngine();

            _mqHelper.PurgeQueue(_config.ADEventQueue);

            if (isRunning)
            {
                StartEngine();
            }
        }

        // -----------------------------------------------------------------------------
        public IWorkerCollection GetWorkerCollection() { return _workerCollection; }

        // -----------------------------------------------------------------------------
        void _StartEngine()
        {
            _workerCollection.Add(_container.Resolve<IWorker>("SatelliteHousemateWorker"));
            _workerCollection.Add(_container.Resolve<IWorker>("ChangeNotifyWorker"));
            _workerCollection.Add(_container.Resolve<IWorker>("SendEvent2ADEWorker"));

            _workerCollection.Start();
        }

        // -----------------------------------------------------------------------------
        void _StopEngine()
        {
            _workerCollection.Clear();
        }
    }
}