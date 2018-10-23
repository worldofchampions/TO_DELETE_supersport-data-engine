using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Configuration;
using System.Threading.Tasks;
using SuperSportDataEngine.Common.Interfaces;

namespace SuperSportDataEngine.Common.Caching
{
    public class Cache : ICache
    {
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly object _lock = new object();
        private readonly string _environmentName;

        private IDatabase Database => _redisConnection.GetDatabase();

        public Cache(ConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
            _environmentName = ConfigurationManager.AppSettings["ENVIRONMENT"];
        }

        private string GetKeyWithEnvironmentPrefix(string key)
        {
            return $"{_environmentName}:{key}";
        }

        public void Add<T>(string key, T cacheObject, TimeSpan? ttl = null, string parentKey = null) where T : class
        {
            key = GetKeyWithEnvironmentPrefix(key);

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
                            new HashEntry("value", JsonConvert.SerializeObject(cacheObject, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })),
                            new HashEntry("parent", parentKey)
                        },
                        CommandFlags.FireAndForget);
                }
                else
                {
                    t.HashSetAsync(key,
                        new HashEntry[] {
                            new HashEntry("value", JsonConvert.SerializeObject(cacheObject, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }))
                        },
                        CommandFlags.FireAndForget);
                }
                t.KeyExpireAsync(key, ttl, CommandFlags.FireAndForget);
            });
        }

        public void Remove(string key)
        {
            key = GetKeyWithEnvironmentPrefix(key);

            ExecuteTransaction(t =>
            {
                t.KeyDeleteAsync(key);
                t.KeyDeleteAsync($"{key}$$children");
            });
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            key = GetKeyWithEnvironmentPrefix(key);

            var hashEntry = await Database.HashGetAllAsync(key);
            if (hashEntry == null)
            {
                return null;
            }

            var dict = hashEntry.ToDictionary();
            if ((dict.ContainsKey("parent") || !dict.ContainsKey("value")) &&
                (!dict.ContainsKey("parent") || !await Database.SetContainsAsync($"{dict["parent"]}$$children", key)))
                return null;

            var value = dict["value"].ToString();
            return JsonConvert.DeserializeObject<T>(value);
        }

        public void SetParentExpiry(string parentKey, TimeSpan ttl)
        {
            Database.KeyExpireAsync($"{parentKey}$$children", ttl, CommandFlags.FireAndForget);
        }

        private void ExecuteTransaction(Action<ITransaction> command)
        {
            var transaction = Database.CreateTransaction();
            command(transaction);
            transaction.Execute();
        }
    }
}