using ADEventSatellite.Configuration;
using ADEventSatellite.Model;
using GK.AppCore.Threads;
using GK.AppCore.Utility;
using Microsoft.Owin.Hosting;
using System;
using System.Diagnostics;

namespace ADEventSatellite
{
    // ================================================================================
    public class ServiceControl : IServiceControl
    {
        readonly IADESLConfig _config;
        readonly IRuntimeInfo _runtimeInfo;
        readonly IEngine _engine;

        readonly Stopwatch _stopWatch = new Stopwatch();

        IDisposable _webapp = null;

        // -----------------------------------------------------------------------------
        public ServiceControl(IADESLConfig config, IRuntimeInfo runtimeInfo, IEngine engine)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _runtimeInfo = Verify.ArgumentNotNull(runtimeInfo, "runtimeInfo");

            _engine = Verify.ArgumentNotNull(engine, "engine");
        }

        // -----------------------------------------------------------------------------
        public void Init()
        {
            _engine.InitEngine();

            StartEngineController();
        }

        // -----------------------------------------------------------------------------
        public void CleanupOnExit()
        {
            StopEngineController();

            _config.Logger.CleanupOnExit();
        }

        // -----------------------------------------------------------------------------
        public IWorkerCollection GetWorkerCollection()
        {
            throw new NotImplementedException("TODO refactor WorkerCollection stuff");
        }

        // -----------------------------------------------------------------------------
        public bool IsRunning()
        {
            return _engine.IsEngineON;
        }

        // -----------------------------------------------------------------------------
        public void Start()
        {
            try
            {
                _stopWatch.Start();

                _engine.StartEngine();
            }
            catch (Exception ex)
            {
                string msg = string.Format("Service failed to START. Reason = [{0}]! ", ex.Message);
                _config.Logger.LogERROR(msg);
            }
        }

        // -----------------------------------------------------------------------------
        public void Stop()
        {
            try
            {
                _engine.StopEngine();

                _stopWatch.Stop();
                _config.Logger.LogINFO(string.Format("Total service up time is [{0}]", UpTime(false).ToString(@"d\.hh\:mm\:ss")));
            }
            catch (Exception ex)
            {
                string msg = string.Format("An error occured during service STOP. Reason = [{0}]. Error is ignored and service is now stopped! ", ex.Message);
                _config.Logger.LogERROR(msg);
            }
        }

        // -----------------------------------------------------------------------------
        public void Reset()
        {
            _engine.Reset();
        }

        // -----------------------------------------------------------------------------
        public TimeSpan UpTime(bool reset)
        {
            return _stopWatch.Elapsed;
        }

        // -----------------------------------------------------------------------------
        void StartEngineController()
        {
            var uri = new Uri(_config.ServiceBaseURL);
            var baseUri = string.Format("http://+:{0}", uri.Port.ToString());

            if (_webapp == null) _webapp = WebApp.Start<Startup>(baseUri);
        }

        // -----------------------------------------------------------------------------
        void StopEngineController()
        {
            if (_webapp != null) _webapp.Dispose();
        }
    }
}