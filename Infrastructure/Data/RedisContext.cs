using StackExchange.Redis;

namespace StreamsMS.Infrastructure.Data
{
    public class RedisContext
    {
        public readonly IDatabase _redisDb;

        public RedisContext(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }
    }
}
