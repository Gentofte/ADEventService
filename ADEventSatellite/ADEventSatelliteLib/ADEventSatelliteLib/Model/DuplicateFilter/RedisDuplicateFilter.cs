using ADEventSatellite.Configuration;
using GK.AD;
using GK.AppCore.Configuration;
using GK.AppCore.Utility;

namespace ADEventSatellite.Model
{
    // ================================================================================
    public class RedisDuplicateFilter : IDuplicateFilter
    {
        object _lock = new object();

        readonly IADESLConfig _config;

        readonly IConfigStoreFactory _configStoreFactory;
        readonly IConfigStore _cache;

        // -----------------------------------------------------------------------------
        public RedisDuplicateFilter(IADESLConfig config, IConfigStoreFactory configStoreFactory)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _configStoreFactory = Verify.ArgumentNotNull(configStoreFactory, "configStoreFactory");

            _cache = _configStoreFactory.CreateConfigStore(_config.ADESLADCacheStoreName);
        }

        // -----------------------------------------------------------------------------
        public void Init() { }

        // -----------------------------------------------------------------------------
        public void CleanupOnExit() { }

        // -----------------------------------------------------------------------------
        public GK.AD.IADObject FilterObject(GK.AD.IADObject adObj)
        {
            if (adObj == null) return adObj;

            if (adObj.objectClass == ObjectClass.group) return adObj;

            var key = adObj.objectGuid;

            bool equal = false;
            bool cacheUpdated = false;

            byte[] newObj = GK.AppCore.Utility.Serializer.SerializeToByteArray<IADObject>(adObj);

            lock (_lock)
            {
                var oldCacheItemAsBytes = _cache.GetBlob(key);

                // If not found ...
                if (!(oldCacheItemAsBytes.Length > 0))
                {
                    // ... write object to cache
                    _cache.SetBlob(key, newObj);
                    cacheUpdated = true;
                }
                else
                {
                    // ... otherwise, evaluate if new and old objects is equal
                    equal = GK.AppCore.Utility.Serializer.IsEqual(newObj, oldCacheItemAsBytes);

                    // If object in cache and new object is different ...
                    if (!equal)
                    {
                        _cache.SetBlob(key, newObj);
                        cacheUpdated = true;
                    }

                    // ... otherwise, do nothing
                }
            }

            if (!cacheUpdated) LogCacheHits(adObj);

            // Return new object or NULL iff the object was found with perfect macth in the cache
            return cacheUpdated ? adObj : null;
        }

        // -----------------------------------------------------------------------------
        public void FlushCache() { }

        // -----------------------------------------------------------------------------
        void LogCacheHits(IADObject adObj)
        {
            if (_config.LogCacheHits)
            {
                _config.Logger.LogINFO(
                    string.Format("EVENT DROPPED: [{0}] <== UNCHANGED object FOUND in cache [{1}]",
                    adObj.ToString(),
                    _cache.Name
                    )
                );
            }
        }
    }
}