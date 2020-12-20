using DotneterWhj.Core.EncodeHelper;
using DotneterWhj.IRepository;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Repository
{
    public class RedisBasketRepository : IRedisBasketRepository
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        
        private readonly IDatabase _database;

        public RedisBasketRepository(ConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;

            _database = _connectionMultiplexer.GetDatabase();
        }

        public T Get<T>(string key)
        {
            var value = _database.StringGet(key);

            if(!string.IsNullOrEmpty(value))
            {
                return SerializeHelper.Deserialize<T>(value);
            }

            return default(T);
        }

        public string GetValue(string key)
        {
            return _database.StringGet(key);
        }

        public void Set(string key, object value, TimeSpan timeSpan)
        {
            _database.StringSet(key, SerializeHelper.Serialize(value), timeSpan);
        }
    }
}
