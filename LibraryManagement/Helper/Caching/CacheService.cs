using Microsoft.Extensions.Caching.Memory;
using Seagull.Core.Helper.Localization;
using Seagull.Core.Helper.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Helper.Caching
{
    public class CacheService : ICacheServicecs
    {
        private const string ResourcesCacheKey = "Resources-cache-key";

        private readonly IMemoryCache _cache;
        private readonly JsonStringLocalizer _resources = new JsonStringLocalizer();

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void CacheResources(string CacheKey)
        {
            if (!_cache.TryGetValue(CacheKey, out List<JsonLocalization> resources))
            {
                _cache.Set(CacheKey, _resources.GetAllStrings(true));
            }
        }

        public void SetCache(string CacheKey, string value)
        {
            object tempValue;

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromDays(1));

            // Look for cache key.
            if (!_cache.TryGetValue(CacheKey, out tempValue))
            {
                // Save data in cache.
                _cache.Set(CacheKey, value, cacheEntryOptions);
            }
            else if (!string.IsNullOrEmpty(value))
            {
                RemoveEntryCache(CacheKey);
                // Update data in cache.
                _cache.Set(CacheKey, value, cacheEntryOptions);
            }

        }

        public object GetCache(string CacheKey)
        {
            object tempValue;
            // Look for cache key.
            if (_cache.TryGetValue(CacheKey, out tempValue))
            {
                // Get cache options.
                return _cache.Get(CacheKey);
            }
            return null;
        }

        public void RemoveEntryCache(string CacheKey)
        {
            //Remove Entry Cache
            _cache.Remove(CacheKey);
        }

    }
}
