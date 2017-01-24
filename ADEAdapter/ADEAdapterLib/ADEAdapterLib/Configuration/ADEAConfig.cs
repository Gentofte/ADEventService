using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Configuration;
using GK.AppCore.Logging;
using GK.AppCore.Utility;
using GK.AppCore.Queues;

namespace ADEAdapterLib.Configuration
{
    // ================================================================================
    public class ADEAConfig : IADEAConfig
    {
        object _lock = new object();

        readonly IAppConfig _appConfig;
        readonly IRuntimeInfo _runtimeInfo;
        readonly ILog _log;

        readonly IConfigStoreFactory _configStoreFactory;
        readonly IConfigStore _configStore;

        readonly string _serviceBaseURL = "http://localhost:8810";

        readonly bool _echoHousemateVisits = false;
        readonly bool _logReceivedNotifications = false;

        readonly string _incomingEventExchange = "";
        readonly string _incomingEventQueue = "";

        // -----------------------------------------------------------------------------
        public ADEAConfig(IAppConfig appConfig, IRuntimeInfo runtimeInfo, ILog log, IConfigStoreFactory configStoreFactory)
        {
            _appConfig = Verify.ArgumentNotNull(appConfig, "appConfig");
            _runtimeInfo = Verify.ArgumentNotNull(runtimeInfo, "runtimeInfo");
            _log = Verify.ArgumentNotNull(log, "log");
            _configStoreFactory = Verify.ArgumentNotNull(configStoreFactory, "configStoreFactory");

            try { _serviceBaseURL = _appConfig.GetAppSettingString("ServiceBaseURL"); }
            catch { }

            try { _echoHousemateVisits = _appConfig.GetAppSettingBool("EchoHousemateVisits"); }
            catch { }

            try { _logReceivedNotifications = _appConfig.GetAppSettingBool("LogNotifications"); }
            catch { }

            _configStore = _configStoreFactory.CreateConfigStore(GetConfigStoreName());

            CreateMQStuff(ref _incomingEventExchange, ref _incomingEventQueue);
        }

        // -----------------------------------------------------------------------------
        public ILog Logger { get { return _log; } }

        // -----------------------------------------------------------------------------
        public string ServiceBaseURL { get { return _serviceBaseURL; } }

        // -----------------------------------------------------------------------------
        public Guid ApplicationID { get { return _runtimeInfo.ApplicationID; } }

        // -----------------------------------------------------------------------------
        public string ApplicationPrefix { get { return _runtimeInfo.ApplicationPrefix; } }

        // -----------------------------------------------------------------------------
        public bool IsProductionEnvironment { get { return _runtimeInfo.IsProductionEnvironment; } }

        // -----------------------------------------------------------------------------
        public bool EchoHousemateVisits { get { return _echoHousemateVisits; } }

        // -----------------------------------------------------------------------------
        public bool LogReceivedNotifications { get { return _logReceivedNotifications; } }

        // -----------------------------------------------------------------------------
        public string IncomingEventExchange { get { return _incomingEventExchange; } }

        // -----------------------------------------------------------------------------
        public string IncomingEventQueue { get { return _incomingEventQueue; } }

        // -----------------------------------------------------------------------------
        string GetConfigStoreName()
        {
            var configStoreName = string.Format(
                "{0}:{1}:Config-{2}",
                _runtimeInfo.ApplicationPrefix,
                _runtimeInfo.ApplicationID.ToString(),
                _runtimeInfo.ApplicationConfiguration
                );

            return configStoreName;
        }

        // -----------------------------------------------------------------------------
        void CreateMQStuff(ref string exchangeName, ref string queueName)
        {
            var _mqPrefix = _runtimeInfo.ApplicationPrefix;
            var _mqPostfix = "v01";
            var _exchangeKeyname = "exchange-key-01";
            var _queueKeyname = "queue-key-01";
            var _incomingEventMQName = "incomingevent";

            var storeKey = "";

            storeKey = BuildStoreKey(_mqPrefix, _incomingEventMQName, _exchangeKeyname);
            exchangeName = GetRabbitMQItemName(storeKey, _mqPrefix, _incomingEventMQName, QueueItemType.Exchange, _mqPostfix);

            storeKey = BuildStoreKey(_mqPrefix, _incomingEventMQName, _queueKeyname);
            queueName = GetRabbitMQItemName(storeKey, _mqPrefix, _incomingEventMQName, QueueItemType.Queue, _mqPostfix);
        }

        // -----------------------------------------------------------------------------
        string BuildStoreKey(string prefix, string shortname, string shortkey)
        {
            return string.Format("{0}-{1}-{2}", prefix, shortname, shortkey);
        }

        // -----------------------------------------------------------------------------
        string GetRabbitMQItemName(string storeKey, string mqPrefix, string mqShortname, QueueItemType mqItemType, string mqPostfix = "")
        {
            mqPostfix = string.IsNullOrWhiteSpace(mqPostfix) ? "" : string.Format("-{0}", mqPostfix.Trim());

            var name = _configStore.Get(storeKey);

            if (string.IsNullOrWhiteSpace(name))
            {
                var sno = (ApplicationID != Guid.Empty) ? ApplicationID.ToString("N") : DateTime.Now.ToString("yyyyMMdd-HHmmss");

                name = string.Format("{0}-{1}-{2}-{3}-{4}{5}",
                    mqPrefix.ToUpperInvariant(),
                    sno.ToUpperInvariant(),
                    _runtimeInfo.ApplicationConfiguration.ToUpperInvariant(),
                    mqShortname,
                    mqItemType.ToString().ToLowerInvariant(),
                    mqPostfix);

                _configStore.Set(storeKey, name);

                string info = string.Format("New MQ itemname [{0}] DEFINED and PERSISTED under key [{1}]", name, storeKey);
                Logger.LogINFO(info);
            }

            return name;
        }
    }
}
