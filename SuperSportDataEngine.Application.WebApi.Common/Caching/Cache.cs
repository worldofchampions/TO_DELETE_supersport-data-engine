using Newtonsoft.Json;
using StackExchange.Redis;
using SuperSportDataEngine.Application.WebApi.Common.Boundaries;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Application.WebApi.Common.Caching
{
    public class Cache : ICache
    {
        private readonly ConnectionMultiplexer _redisConnection;

        private IDatabase Database
        {
            get
            {
                return _redisConnection.GetDatabase();
            }
        }

        public Cache(ConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
        }

        public void Add<T>(string key, T cacheObject, TimeSpan? ttl = null, string parentKey = null) where T : class
        {
            if(ttl == null)
            {
                ttl = TimeSpan.FromSeconds(20);
            }

            ExecuteTransaction(t =>
            {
                if (parentKey != null)
                {
                    t.SetAddAsync($"{parentKey}$$children", key, CommandFlags.FireAndForget);
                }
                if (parentKey != null)
                {
                    t.HashSetAsync(key,
                        new HashEntry[] {
                        new HashEntry("value", JsonConvert.SerializeObject(cacheObject)),
                        new HashEntry("parent", parentKey)
                        },
                        CommandFlags.FireAndForget);
                }
                else
                {
                    t.HashSetAsync(key,
                        new HashEntry[] {
                        new HashEntry("value", JsonConvert.SerializeObject(cacheObject))
                        },
                        CommandFlags.FireAndForget);
                }
                t.KeyExpireAsync(key, ttl, CommandFlags.FireAndForget);
            });
        }

        public void Remove(string key)
        {
            ExecuteTransaction(t =>
            {
                t.KeyDeleteAsync(key);
                t.KeyDeleteAsync($"{key}$$children");
            });
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            try
            {
                var hashEntry = await Database.HashGetAllAsync(key);
                if (hashEntry == null)
                {
                    return null;
                }

                var dict = hashEntry.ToDictionary();
                if ((!dict.ContainsKey("parent") && dict.ContainsKey("value")) ||
                    (dict.ContainsKey("parent") && await Database.SetContainsAsync($"{dict["parent"]}$$children", key)))
                {
                    return JsonConvert.DeserializeObject<T>(dict["value"]);
                }

                return null;
            }
            catch (RedisServerException) { }

            return null;
        }

        public void SetParentExpiry(string parentKey, TimeSpan ttl)
        {
            Database.KeyExpireAsync($"{parentKey}$$children", ttl, CommandFlags.FireAndForget);
        }

        private bool ExecuteTransaction(Action<ITransaction> command)
        {
            var transaction = _redisConnection.GetDatabase().CreateTransaction();
            command(transaction);
            return transaction.Execute();
        }
    }
}