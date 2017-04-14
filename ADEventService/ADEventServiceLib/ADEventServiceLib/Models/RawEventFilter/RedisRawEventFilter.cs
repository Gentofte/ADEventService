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
        public GK.AD.IADObject FilterObject(GK.AD.IADObject adObj, DateTime changeTS)
        {
            if (adObj == null) return adObj;

            // Never filter out groups ...
            if (adObj.objectClass == ObjectClass.group) return adObj;

            var key = adObj.objectGuid;

            byte[] newObj = Serializer.SerializeToByteArray(adObj);

            CacheItem newCacheItem = new CacheItem() { Value = newObj, ChangeTS = changeTS };
            byte[] newCacheItemAsBytes = Serializer.SerializeToByteArray(newCacheItem);

            var oldCachedItemAsBytes = new byte[0];
            CacheItem oldCachedItem = null;

            if (_config.EnableCacheLocks)
            {
                lock (_lock)
                {
                    oldCachedItemAsBytes = _cache.GetBlob(key);
                    _cache.SetBlob(key, newCacheItemAsBytes);

                    // If object isn't found in cache, it's already saved in cache, so just return object (should be handled further down the line)
                    if (oldCachedItemAsBytes.Length == 0) return adObj;

                    oldCachedItem = Serializer.DeSerializeFromByteArray<CacheItem>(oldCachedItemAsBytes);
                    if (!(oldCachedItem.ChangeTS < newCacheItem.ChangeTS))
                    {
                        // Restore previous (old) cache item
                        _cache.SetBlob(key, oldCachedItemAsBytes);
                        _config.Logger.LogWARNING(string.Format("Timestamp of new object [{0}] is OLDER than timestamp of cached object [{1}]", newCacheItem.ChangeTS, oldCachedItem.ChangeTS));
                    }
                }
            }
            else
            {
                oldCachedItemAsBytes = _cache.GetBlob(key);
                _cache.SetBlob(key, newCacheItemAsBytes);

                // If object isn't found in cache, it's already saved in cache, so just return object (should be handled further down the line)
                if (oldCachedItemAsBytes.Length == 0) return adObj;

                oldCachedItem = Serializer.DeSerializeFromByteArray<CacheItem>(oldCachedItemAsBytes);
                if (!(oldCachedItem.ChangeTS < newCacheItem.ChangeTS))
                {
                    // Restore previous (old) cache item
                    _cache.SetBlob(key, oldCachedItemAsBytes);
                    _config.Logger.LogWARNING(string.Format("Timestamp of new object [{0}] is OLDER than timestamp of cached object [{1}]", newCacheItem.ChangeTS, oldCachedItem.ChangeTS));
                }
            }

            // Otherwise, object was found in cache. If new object and existing object has the same value,
            // return null to indicate cache hit ie. stop further processing
            if (Serializer.IsEqual(newObj, oldCachedItem.Value))
            {
                LogCacheHits(adObj);
                return null;
            }

            // Objects are different ==> update cache and return object for further processing
            return adObj;
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
