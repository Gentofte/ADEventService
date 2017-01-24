using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Configuration;
using GK.AppCore.Utility;

using GK.AD;

using ADEventService.Configuration;

namespace ADEventService.Models
{
    // ================================================================================
    public class RedisRawEventFilter : IRawEventFilter
    {
        object _lock = new object();

        readonly IADESConfig _config;

        readonly IConfigStoreFactory _configStoreFactory;
        readonly IConfigStore _cache;

        // -----------------------------------------------------------------------------
        public RedisRawEventFilter(IADESConfig config, IConfigStoreFactory configStoreFactory)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _configStoreFactory = Verify.ArgumentNotNull(configStoreFactory, "configStoreFactory");

            _cache = _configStoreFactory.CreateConfigStore(_config.ADESADCacheStoreName);
        }

        // -----------------------------------------------------------------------------
        public void Init()
        {
        }

        // -----------------------------------------------------------------------------
        public void CleanupOnExit()
        {
        }

        // -----------------------------------------------------------------------------
        public IADObject FilterObject(IADObject adObj, DateTime changeTS)
        {
            if (IncludeInCacheHandling(adObj) == false)
                return adObj;

            var key = adObj.objectGuid;

            bool found = false;
            bool equal = false;
            bool cacheUpdated = false;

            byte[] newObj = Serializer.SerializeToByteArray(adObj);

            CacheItem newCacheItem = new CacheItem() { Value = newObj, ChangeTS = changeTS };
            byte[] newCacheItemAsBytes = Serializer.SerializeToByteArray(newCacheItem);

            lock (_lock)
            {
                // Check if object already exists in cache
                byte[] oldCacheItemAsBytes;
                CacheItem oldCacheItem = null;

                oldCacheItemAsBytes = _cache.GetBlob(key);

                // If not found ...
                if (!found)
                {
                    // ... write object to cache
                    _cache.SetBlob(key, newCacheItemAsBytes);
                    cacheUpdated = true;
                }
                else
                {
                    // ... otherwise, evaluate if new and old objects is equal
                    oldCacheItem = Serializer.DeSerializeFromByteArray<CacheItem>(oldCacheItemAsBytes);
                    equal = Serializer.IsEqual(newObj, oldCacheItem.Value);

                    // If object in cache and new object is different AND new object is newer than the one in cache ...
                    if (!equal && (newCacheItem.ChangeTS > oldCacheItem.ChangeTS))
                    {
                        _cache.SetBlob(key, newCacheItemAsBytes);
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
        public void FlushCache()
        {
        }

        // -----------------------------------------------------------------------------
        bool IncludeInCacheHandling(IADObject adObj)
        {
            if (adObj.objectClass == ObjectClass.user) return _config.CacheUserEvents;
            if (adObj.objectClass == ObjectClass.group) return _config.CacheGroupEvents;
            if (adObj.objectClass == ObjectClass.organizationalUnit) return _config.CacheOUEvents;

            return false;
        }

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
