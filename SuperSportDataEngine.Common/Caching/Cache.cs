using Newtonsoft.Json;
using StackExchange.Redis;
using SuperSportDataEngine.Application.WebApi.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Common.Caching
{
    public class Cache : ICache
    {
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly object _lock = new object();

        private IDatabase Database => _redisConnection.GetDatabase();

        public Cache(ConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
        }

        public void Add<T>(string key, T cacheObject, TimeSpan? ttl = null, string parentKey = null) where T : class
        {
            if (ttl == null)
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

        private void ExecuteTransaction(Action<ITransaction> command)
        {
            lock (_lock)
            {
                var transaction = Database.CreateTransaction();
                command(transaction);
                transaction.Execute();
            }
        }
    }
}