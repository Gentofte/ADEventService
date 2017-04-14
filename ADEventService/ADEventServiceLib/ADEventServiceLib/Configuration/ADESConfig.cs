using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GK.AppCore.Configuration;
using GK.AppCore.Logging;
using GK.AppCore.Utility;
using GK.AppCore.Queues;

namespace ADEventService.Configuration
{
    // ================================================================================
    public class ADESConfig : IADESConfig
    {
        object _lock = new object();

        readonly IAppConfig _appConfig;
        readonly IRuntimeInfo _runtimeInfo;
        readonly ILog _log;

        readonly string _serviceBaseURL = "http://localhost:8300";

        readonly int _housemateVisitIntervalInSec = 15;
        readonly bool _echoHousemateVisits = false;

        readonly string _rawEventExchange = "";
        readonly string _rawEventQueue = "";

        readonly string _filteredEventExchange = "";
        readonly string _eventEchoQueue = "";

        readonly bool _logCacheHits = false;
        readonly bool _dropADEEvents = false;
        readonly bool _logNULLNotifications = false;
        readonly bool _logRawEventsReceived =false;
        readonly bool _logEventsTransmitted = false;
        readonly bool _enableADxSampleSubscription = false;
        readonly bool _enableCacheLocks = false;

        const string _configStoreDataFolderName = "Config";

        const string _configStoreFolderPathAppSettingKey = "ConfigStoreFolderPath";
        const string _subscriptionsConfigFilenameAppSettingKey = "SubscriptionsConfigFilename";

        string _configFolderPath = "";
        string _subscriptionsConfigFilename = "";

        readonly string _aDESADCacheStoreName = "";
        readonly string _aDESConfigStoreName = "";

        readonly IConfigStoreFactory _configStoreFactory;
        readonly IConfigStore _configStore;

        readonly string _appPrefix = "ADESVC";

        const string _postfix = "v01";
        const string _exchangeKeyname = "ExchangeName-key-01";
        const string _queueKeyname = "QueueName-key-01";

        const string _rawEventMQName = "rawevent";
        const string _filteredEventExchangeName = "filteredevent";
        const string _echoEventQueueName = "echo";

        readonly IMQHelper _mqHelper;

        // -----------------------------------------------------------------------------
        public ADESConfig(IAppConfig appConfig, IRuntimeInfo runtimeInfo, ILog log, IConfigStoreFactory configStoreFactory, IMQHelper mqHelper)
        {
            _appConfig = Verify.ArgumentNotNull(appConfig, "appConfig");
            _runtimeInfo = Verify.ArgumentNotNull(runtimeInfo, "runtimeInfo");
            _log = Verify.ArgumentNotNull(log, "log");
            _configStoreFactory = Verify.ArgumentNotNull(configStoreFactory, "configStoreFactory");
            _mqHelper = Verify.ArgumentNotNull(mqHelper, "mqHelper");

            OneTimeInit();

            try { _serviceBaseURL = _appConfig.GetAppSettingString("ServiceBaseURL"); }
            catch { }

            try { _housemateVisitIntervalInSec = _appConfig.GetAppSettingInt("HousemateVisitIntervalInSec"); }
            catch { }
            try { _echoHousemateVisits = _appConfig.GetAppSettingBool("EchoHousemateVisits"); }
            catch { }

            try { _logCacheHits = _appConfig.GetAppSettingBool("LogCacheHits"); }
            catch { }
            try { _dropADEEvents = _appConfig.GetAppSettingBool("DropADEEvents"); }
            catch { }

            try { _logNULLNotifications = _appConfig.GetAppSettingBool("LogNULLNotifications"); }
            catch { }

            try { _logRawEventsReceived = _appConfig.GetAppSettingBool("LogRawEventsReceived"); }
            catch { }

            try { _logEventsTransmitted = _appConfig.GetAppSettingBool("LogEventsTransmitted"); }
            catch { }

            try { _enableADxSampleSubscription = _appConfig.GetAppSettingBool("EnableADxSampleSubscription"); }
            catch { }

            try { _enableCacheLocks = _appConfig.GetAppSettingBool("EnableCacheLocks"); }
            catch { }

            _appPrefix = (ApplicationPrefix.Length > 0) ? ApplicationPrefix : _appPrefix;

            _aDESADCacheStoreName = string.Format("{0}:ObjectCache-{1}-{2}", _appPrefix, _runtimeInfo.ApplicationConfiguration, _postfix);
            _aDESConfigStoreName = string.Format("{0}:Config-{1}-{2}", _appPrefix, _runtimeInfo.ApplicationConfiguration, _postfix);

            _configStore = _configStoreFactory.CreateConfigStore(_aDESConfigStoreName);

            var key = "";

            key = BuildKey(_appPrefix, _rawEventMQName, _exchangeKeyname);
            _rawEventExchange = GetRabbitMQItemName(key, _appPrefix, _rawEventMQName, QueueItemType.Exchange, _postfix);
            key = BuildKey(_appPrefix, _rawEventMQName, _queueKeyname);
            _rawEventQueue = GetRabbitMQItemName(key, _appPrefix, _rawEventMQName, QueueItemType.Queue, _postfix);

            key = BuildKey(_appPrefix, _filteredEventExchangeName, _exchangeKeyname);
            _filteredEventExchange = GetRabbitMQItemName(key, _appPrefix, _filteredEventExchangeName, QueueItemType.Exchange, _postfix);
            key = BuildKey(_appPrefix, _echoEventQueueName, _queueKeyname);
            _eventEchoQueue = GetRabbitMQItemName(key, _appPrefix, _echoEventQueueName, QueueItemType.Queue, _postfix);
        }

        // -----------------------------------------------------------------------------
        public ILog Logger { get { return _log; } }

        // -----------------------------------------------------------------------------
        public string ServiceBaseURL { get { return _serviceBaseURL; } }

        // -----------------------------------------------------------------------------
        public int HousemateVisitIntervalInSec { get { return _housemateVisitIntervalInSec; } }
        // -----------------------------------------------------------------------------
        public bool EchoHousemateVisits { get { return _echoHousemateVisits; } }

        // -----------------------------------------------------------------------------
        public string RawEventExchange { get { return _rawEventExchange; } }
        // -----------------------------------------------------------------------------
        public string RawEventQueue { get { return _rawEventQueue; } }

        // -----------------------------------------------------------------------------
        public string FilteredEventExchange { get { return _filteredEventExchange; } }
        // -----------------------------------------------------------------------------
        public string EventEchoQueue { get { return _eventEchoQueue; } }

        // -----------------------------------------------------------------------------
        public bool CacheUserEvents { get { return true; } }
        // -----------------------------------------------------------------------------
        public bool CacheGroupEvents { get { return false; } }
        // -----------------------------------------------------------------------------
        public bool CacheOUEvents { get { return true; } }

        // -----------------------------------------------------------------------------
        public bool LogCacheHits { get { return _logCacheHits; } }
        // -----------------------------------------------------------------------------
        public bool DropADEEvents { get { return _dropADEEvents; } }

        // -----------------------------------------------------------------------------
        public bool LogNULLNotifications { get { return _logNULLNotifications; } }

        // -----------------------------------------------------------------------------
        public bool LogRawEventsReceived { get { return _logRawEventsReceived; } }

        // -----------------------------------------------------------------------------
        public bool LogEventsTransmitted { get { return _logEventsTransmitted; } }

        // -----------------------------------------------------------------------------
        public bool EnableADxSampleSubscription { get { return _enableADxSampleSubscription; } }

        // -----------------------------------------------------------------------------
        public bool EnableCacheLocks { get { return _enableCacheLocks; } }

        // -----------------------------------------------------------------------------
        public Guid ApplicationID { get { return _runtimeInfo.ApplicationID; } }
        // -----------------------------------------------------------------------------
        public string ApplicationPrefix { get { return _runtimeInfo.ApplicationPrefix; } }

        // -----------------------------------------------------------------------------
        public string SubscriptionsConfigFilename { get { return Path.Combine(_configFolderPath, _subscriptionsConfigFilename); } }
        // -----------------------------------------------------------------------------
        public string ADESADCacheStoreName { get { return _aDESADCacheStoreName; } }
        // -----------------------------------------------------------------------------
        public string ADESConfigStoreName { get { return _aDESConfigStoreName; } }

        // -----------------------------------------------------------------------------
        public IConfigStore ConfigStore { get { return _configStore; } }

        // -----------------------------------------------------------------------------
        public void Reset()
        {
            // delete all queues and exchanges
            //_mqHelper.DeleteQueue(EventEchoQueue);
            //_mqHelper.DeleteQueue(RawEventQueue);

            // delete all konfig and cache in redis

            //
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

                string info = string.Format("MQ name [{0}] DEFINED/SAVED under key [{1}]", name, key);
                Logger.LogINFO(info);
            }

            return name;
        }

        // -----------------------------------------------------------------------------
        void OneTimeInit()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(exePath));

            _subscriptionsConfigFilename = _appConfig.GetAppSettingString(_subscriptionsConfigFilenameAppSettingKey, false);
            if (_subscriptionsConfigFilename.Length == 0)
            {
                _subscriptionsConfigFilename = string.Format("SubscriptionsConfig-{0}.xml", ApplicationID.ToString().ToUpperInvariant());
            }
            VerifyConfigStore();
        }

        // -----------------------------------------------------------------------------
        void VerifyConfigStore()
        {
            _configFolderPath = _appConfig.GetAppSettingString(_configStoreFolderPathAppSettingKey, false);
            if (_configFolderPath == string.Empty)
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                exePath = Path.GetDirectoryName(exePath);

                _configFolderPath = exePath + @"\" + _configStoreDataFolderName;
            }

            if (!Directory.Exists(_configFolderPath))
            {
                Directory.CreateDirectory(_configFolderPath);
            }
        }

    }
}
