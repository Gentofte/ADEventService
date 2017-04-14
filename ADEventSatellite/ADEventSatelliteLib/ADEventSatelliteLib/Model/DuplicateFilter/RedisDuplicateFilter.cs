using GK.AD;
using GK.AppCore.Configuration;
using GK.AppCore.Utility;

using ADEventSatellite.Configuration;

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

            // Never filter out groups ...
            if (adObj.objectClass == ObjectClass.group) return adObj;

            var key = adObj.objectGuid;
            byte[] newItemAsBytes = Serializer.SerializeToByteArray(adObj);

            var oldCachedItemAsBytes = new byte[0];

            if (_config.EnableCacheLocks)
            {
                lock (_lock)
                {
                    oldCachedItemAsBytes = _cache.GetBlob(key);

                    // Save the new object in cache - it makes no difference, if the same (by value) object is there already,
                    // except for a minimal waste of clockcycles...  

                    _cache.SetBlob(key, newItemAsBytes);
                }
            }
            else
            {
                oldCachedItemAsBytes = _cache.GetBlob(key);

                // Save the new object in cache - it makes no difference, if the same (by value) object is there already,
                // except for a minimal waste of clockcycles...  

                _cache.SetBlob(key, newItemAsBytes);
            }

            // If object isn't found in cache, then save it in cache and return object (object should be handled further down the line)
            if (oldCachedItemAsBytes.Length == 0) return adObj;

            // Otherwise, object was found in cache. If new object and existing object has the same value,
            // return null to indicate cache hit ie. stop further processing
            if (Serializer.IsEqual(newItemAsBytes, oldCachedItemAsBytes))
            {
                LogCacheHits(adObj);
                return null;
            }

            // Objects are different ==> update cache and return object for further processing
            return adObj;
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