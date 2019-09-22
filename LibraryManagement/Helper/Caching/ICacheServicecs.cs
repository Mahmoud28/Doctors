using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Helper.Caching
{
    public interface ICacheServicecs
    {
        void SetCache(string CacheKey, string value);
        object GetCache(string CacheKey);
        void CacheResources(string CacheKey);
    }
}
