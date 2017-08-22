using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Common.Boundaries
{
    public interface ICache
    {

        void Add<T>(string key, T cacheObject, TimeSpan? ttl = null, string parentKey = null) where T : class;

        void Remove(string key);

        void SetParentExpiry(string parentKey, TimeSpan ttl);

        Task<T> GetAsync<T>(string key) where T : class;

    }
}