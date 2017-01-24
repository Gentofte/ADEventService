using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using RaptorDB;

using GK.AppCore.Utility;
using GK.AD;

using ADEventService.Configuration;
using ADEventService.Utility;

namespace ADEventService.Model
{
    // ================================================================================
    public class RawEventFilter : IRawEventFilter
    {
        object _lock = new object();

        readonly IADESConfig _config;
        readonly IRuntimeInfo _runtimeInfo;

        RaptorDB<Guid> _db;

        bool _logDroppedEvents = false;

        // -----------------------------------------------------------------------------
        public RawEventFilter(IADESConfig config, IRuntimeInfo runtimeInfo)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _runtimeInfo = Verify.ArgumentNotNull(runtimeInfo, "runtimeInfo");
        }

        // -----------------------------------------------------------------------------
        public void Init()
        {
            lock (_lock)
            {
                if (_db == null)
                {
                    string dbStore = Path.Combine(_runtimeInfo.ApplicationCurrentDirectory, @"Cache", @"ADESADObjects");
                    _db = RaptorDB<Guid>.Open(dbStore, false);
                }
            }
        }

        // -----------------------------------------------------------------------------
        public void CleanupOnExit()
        {
            lock (_lock)
            {
                if (_db != null)
                {
                    _db.Shutdown();
                    _db = null;
                }
            }
        }

        // -----------------------------------------------------------------------------
        public GK.AD.IADObject FilterObject(GK.AD.IADObject adObj, DateTime changeTS)
        {
            if (IncludeInCacheHandling(adObj) == false)
                return adObj;

            Guid key = adObj.objectGuid;

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

                found = _db.Get(key, out oldCacheItemAsBytes);

                // If not found ...
                if (!found)
                {
                    // ... write object to cache
                    _db.Set(key, newCacheItemAsBytes);
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
                        _db.Set(key, newCacheItemAsBytes);
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
            throw new NotImplementedException();
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
            if (_logDroppedEvents)
            {
                string info = string.Format("AD change EVENT DROPPED (no real change occured) : [{0}]", ObjectTagger.GetObjectTag(adObj));
                _config.Logger.LogINFO(info);
            }
        }
    }

}
