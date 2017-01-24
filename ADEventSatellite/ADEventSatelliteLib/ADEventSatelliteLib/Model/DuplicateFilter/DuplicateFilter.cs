using ADEventSatellite.Configuration;
using ADEventSatellite.Utility;
using GK.AD;
using GK.AppCore.Utility;
using RaptorDB;
using System;
using System.IO;

namespace ADEventSatellite.Model
{
    // ================================================================================
    public class DuplicateFilter : IDuplicateFilter
    {
        object _lock = new object();

        readonly IADESLConfig _config;
        readonly IRuntimeInfo _runtimeInfo;

        RaptorDB<Guid> _db;

        // -----------------------------------------------------------------------------
        public DuplicateFilter(IADESLConfig config, IRuntimeInfo runtimeInfo)
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
                    string dbStore = Path.Combine(_runtimeInfo.ApplicationCurrentDirectory, @"Cache", @"SatelliteADObjects");
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
        public IADObject FilterObject(IADObject adObj)
        {
            if (adObj == null) return adObj;

            if (adObj.objectClass == ObjectClass.group) return adObj;

            Guid key = adObj.objectGuid;

            bool found = false;
            bool equal = false;
            bool cacheUpdated = false;

            byte[] newObj = GK.AppCore.Utility.Serializer.SerializeToByteArray<IADObject>(adObj);

            lock (_lock)
            {
                // Check if object already exists in cache
                byte[] oldCacheItemAsBytes;
                found = _db.Get(key, out oldCacheItemAsBytes);

                // If not found ...
                if (!found)
                {
                    // ... write object to cache
                    _db.Set(key, newObj);
                    cacheUpdated = true;
                }
                else
                {
                    // ... otherwise, evaluate if new and old objects is equal
                    equal = GK.AppCore.Utility.Serializer.IsEqual(newObj, oldCacheItemAsBytes);

                    // If object in cache and new object is different ...
                    if (!equal)
                    {
                        _db.Set(key, newObj);
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
        void LogCacheHits(IADObject adObj)
        {
            if (_config.LogCacheHits)
            {
                string info = string.Format("AD change FOUND in cache ==> EVENT DROPPED : [{0}]", ObjectTagger.GetObjectTag(adObj));
                _config.Logger.LogINFO(info);
            }
        }

        // -----------------------------------------------------------------------------
        public void FlushCache()
        {
            throw new NotImplementedException();
        }
    }

}
