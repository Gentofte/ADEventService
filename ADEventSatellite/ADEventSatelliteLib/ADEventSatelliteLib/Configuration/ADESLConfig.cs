using GK.AppCore.Configuration;
using GK.AppCore.Logging;
using GK.AppCore.Queues;
using GK.AppCore.Utility;
using System;

namespace ADEventSatellite.Configuration
{
    // ================================================================================
    public class ADESLConfig : IADESLConfig
    {
        object _lock = new object();

        readonly IAppConfig _appConfig;
        readonly IRuntimeInfo _runtimeInfo;
        readonly ILog _log;

        readonly bool _isProductionEnvironment = false;

        readonly string _serviceBaseURL = "http://localhost:8333";

        readonly int _housemateVisitIntervalInSec = 15;
        readonly bool _echoHousemateVisits = false;

        readonly string _ADEventServiceURL = "http://localhost:8300/api/v1/rawevent";

        readonly int _changeNotifierLifetimeInSec = 60;
        readonly bool _logChangeNotifierShifts = false;

        readonly bool _logNotificationEvents = false;

        readonly string _ADEventExchange = "";
        readonly string _ADEventQueue = "";

        readonly bool _logCacheHits = false;
        readonly bool _enableCacheLocks = false;
        readonly bool _dropADEEvents = false;
        readonly bool _logBeforeCache = false;

        readonly bool _logEventsTransmitted = false;

        readonly string _aDESLADCacheStoreName = "";
        readonly string _aDESLConfigStoreName = "";

        readonly IConfigStoreFactory _configStoreFactory;
        readonly IConfigStore _configStore;

        string _prefix = "ADESATL";

        const string _postfix = "v01";
        const string _exchangeKeyname = "ExchangeName-key-01";
        const string _queueKeyname = "QueueName-key-01";

        const string _rawEventMQName = "rawevent";

        // -----------------------------------------------------------------------------
        public ADESLConfig(IAppConfig appConfig, IRuntimeInfo runtimeInfo, ILog log, IConfigStoreFactory configStoreFactory)
        {
            _appConfig = Verify.ArgumentNotNull(appConfig, "appConfig");
            _runtimeInfo = Verify.ArgumentNotNull(runtimeInfo, "runtimeInfo");
            _log = Verify.ArgumentNotNull(log, "log");
            _configStoreFactory = Verify.ArgumentNotNull(configStoreFactory, "configStoreFactory");

            try { _serviceBaseURL = _appConfig.GetAppSettingString("ServiceBaseURL"); }
            catch { }

            try { _housemateVisitIntervalInSec = _appConfig.GetAppSettingInt("HousemateVisitIntervalInSec"); }
            catch { }
            try { _echoHousemateVisits = _appConfig.GetAppSettingBool("EchoHousemateVisits"); }
            catch { }

            try { _ADEventServiceURL = _appConfig.GetAppSettingString("ADEventServiceURL"); }
            catch { }

            try { _changeNotifierLifetimeInSec = _appConfig.GetAppSettingInt("ChangeNotifierLifetimeInSec"); }
            catch { }
            try { _logChangeNotifierShifts = _appConfig.GetAppSettingBool("LogChangeNotifierShifts"); }
            catch { }
            try { _logNotificationEvents = _appConfig.GetAppSettingBool("LogNotificationEvents"); }
            catch { }

            try { _logCacheHits = _appConfig.GetAppSettingBool("LogCacheHits"); }
            catch { }
            try { _enableCacheLocks = _appConfig.GetAppSettingBool("EnableCacheLocks"); }
            catch { }
            try { _dropADEEvents = _appConfig.GetAppSettingBool("DropADEEvents"); }
            catch { }
            try { _logBeforeCache = _appConfig.GetAppSettingBool("LogBeforeCache"); }
            catch { }

            try { _logEventsTransmitted = _appConfig.GetAppSettingBool("LogEventsTransmitted"); }
            catch { }

            _prefix = !string.IsNullOrEmpty(_runtimeInfo.ApplicationPrefix) ? _runtimeInfo.ApplicationPrefix : _prefix;

            _aDESLADCacheStoreName = string.Format("{0}:{1}:ObjectCache-{2}", _prefix, ApplicationID.ToString("N"), runtimeInfo.ApplicationConfiguration);
            _aDESLConfigStoreName = string.Format("{0}:{1}:Config-{2}", _prefix, ApplicationID.ToString("N"), runtimeInfo.ApplicationConfiguration);

            _configStore = _configStoreFactory.CreateConfigStore(_aDESLConfigStoreName);

            var key = "";

            key = BuildKey(_prefix, _rawEventMQName, _exchangeKeyname);
            _ADEventExchange = GetRabbitMQItemName(key, _prefix, _rawEventMQName, QueueItemType.Exchange, _postfix);
            key = BuildKey(_prefix, _rawEventMQName, _queueKeyname);
            _ADEventQueue = GetRabbitMQItemName(key, _prefix, _rawEventMQName, QueueItemType.Queue, _postfix);
        }

        // -----------------------------------------------------------------------------
        public ILog Logger { get { return _log; } }

        // -----------------------------------------------------------------------------
        public bool IsProductionEnvironment { get { return _isProductionEnvironment; } }

        // -----------------------------------------------------------------------------
        public string ServiceBaseURL { get { return _serviceBaseURL; } }

        // -----------------------------------------------------------------------------
        public int HousemateVisitIntervalInSec { get { return _housemateVisitIntervalInSec; } }

        // -----------------------------------------------------------------------------
        public bool EchoHousemateVisits { get { return _echoHousemateVisits; } }

        // -----------------------------------------------------------------------------
        public string ADEventServiceURL { get { return _ADEventServiceURL; } }

        // -----------------------------------------------------------------------------
        public int ChangeNotifierLifetimeInSec { get { return _changeNotifierLifetimeInSec; } }

        // -----------------------------------------------------------------------------
        public bool LogChangeNotifierShifts { get { return _logChangeNotifierShifts; } }

        // -----------------------------------------------------------------------------
        public bool LogNotificationEvents { get { return _logNotificationEvents; } }

        // -----------------------------------------------------------------------------
        public string ADEventExchange { get { return _ADEventExchange; } }

        // -----------------------------------------------------------------------------
        public string ADEventQueue { get { return _ADEventQueue; } }

        // -----------------------------------------------------------------------------
        public bool LogCacheHits { get { return _logCacheHits; } }

        // -----------------------------------------------------------------------------
        public bool EnableCacheLocks { get { return _enableCacheLocks; } }

        // -----------------------------------------------------------------------------
        public bool DropADEEvents { get { return _dropADEEvents; } }

        // -----------------------------------------------------------------------------
        public bool LogBeforeCache { get { return _logBeforeCache; } }

        // -----------------------------------------------------------------------------
        public bool LogEventsTransmitted { get { return _logEventsTransmitted; } }

        // -----------------------------------------------------------------------------
        public Guid ApplicationID { get { return _runtimeInfo.ApplicationID; } }

        // -----------------------------------------------------------------------------
        public string ADESLADCacheStoreName { get { return _aDESLADCacheStoreName; } }

        // -----------------------------------------------------------------------------
        public string ADESLConfigStoreName { get { return _aDESLConfigStoreName; } }

        // -----------------------------------------------------------------------------
        public void Reset()
        {
            // delete all queues and exchanges
            //_mqHelper.DeleteQueue(EventEchoQueue);
            //_mqHelper.DeleteQueue(RawEventQueue);

            // delete all konfig and cache in redis
        }

        // -----------------------------------------------------------------------------
        string BuildKey(string prefix, string shortname, string shortkey)
        {
            return string.Format("{0}-{1}-{2}", prefix, shortname, shortkey);
        }

        // -----------------------------------------------------------------------------
        string GetRabbitMQItemName(string key, string prefix, string shortname, QueueItemType itemType, string postfix = "")
        {
            postfix = string.IsNullOrWhiteSpace(postfix) ? "" : string.Format("-{0}", postfix.Trim());

            var name = _configStore.Get(key);

            if (string.IsNullOrWhiteSpace(name))
            {
                var sno = (ApplicationID != Guid.Empty) ? ApplicationID.ToString("N") : DateTime.Now.ToString("yyyyMMdd-HHmmss");

                name = string.Format("{0}-{1}-{2}-{3}-{4}{5}",
                    prefix.ToUpperInvariant(),
                    sno.ToUpperInvariant(),
                    _runtimeInfo.ApplicationConfiguration.ToUpperInvariant(),
                    shortname,
                    itemType.ToString().ToLowerInvariant(),
                    postfix);

                _configStore.Set(key, name);

                string info = string.Format("New MQ itemname [{0}] DEFINED and PERSISTED under key [{1}]", name, key);
                Logger.LogINFO(info);
            }

            return name;
        }
    }
}