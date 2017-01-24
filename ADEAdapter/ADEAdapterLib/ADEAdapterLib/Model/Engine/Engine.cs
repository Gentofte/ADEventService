using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

using Microsoft.Owin.Hosting;

using GK.AppCore.Threads;
using GK.AppCore.Utility;

using ADEAdapterLib.Configuration;

namespace ADEAdapterLib.Model
{
    // ================================================================================
    public class Engine : IEngine
    {
        readonly IADEAConfig _config;
        readonly IWorkerCollection _workerCollection;
        readonly IUnityContainer _container;
        readonly IRuntimeInfo _runtimeInfo;

        // -----------------------------------------------------------------------------
        public Engine(IADEAConfig config, IWorkerCollection workerCollection, IUnityContainer container, IRuntimeInfo runtimeInfo)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _workerCollection = Verify.ArgumentNotNull(workerCollection, "workerCollection");
            _container = Verify.ArgumentNotNull(container, "container");
            _runtimeInfo = Verify.ArgumentNotNull(runtimeInfo, "runtimeInfo");
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
        public IWorkerCollection GetWorkerCollection() { return _workerCollection; }

        // -----------------------------------------------------------------------------
        public string GetEngineInfo()
        {
            return string.Format(
                "ApplID=[{0}], ApplPrefix=[{1}], PROD=[{2}], service baseurl=[{3}]",
                _config.ApplicationID.ToString().ToUpperInvariant(),
                _config.ApplicationPrefix,
                _config.IsProductionEnvironment.ToString().ToUpperInvariant(),
                _config.ServiceBaseURL
                );
        }

        // -----------------------------------------------------------------------------
        void _StartEngine()
        {
            _workerCollection.Add(_container.Resolve<IWorker>("HousemateWorker"));
            _workerCollection.Add(_container.Resolve<IWorker>("IncomingEventWorker"));

            _workerCollection.Start();

            _config.Logger.LogINFO(GetEngineInfo());
        }

        // -----------------------------------------------------------------------------
        void _StopEngine()
        {
            _workerCollection.Clear();
        }
        
    }
}
