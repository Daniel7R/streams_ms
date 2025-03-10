using StackExchange.Redis;
using System.Text.RegularExpressions;

namespace StreamsMS.Infrastructure.Services
{
    public class RedisViewerService
    {
        private const string PREFIX_KEY = "stream";
        private const int LIMIT_FREE_VIEWERS = 2;//CAMBIAR, SOLO 2 PARA PRUEBAS
        private readonly IDatabase _redisDatabase;
        
        public RedisViewerService(IConnectionMultiplexer redis)
        {
            _redisDatabase = redis.GetDatabase();
        }

        public async Task<bool> CanJoinStreamAsync(int idMath, int maxViewers)
        {
            int currentViewers = (int)await _redisDatabase.StringGetAsync($"{PREFIX_KEY}-{idMath}-viewers:");
            return currentViewers < maxViewers;
        }

        public async Task<int> GetViewerCountAsync(int idMatch)
        {
            return (int)await _redisDatabase.StringGetAsync($"{PREFIX_KEY}-{idMatch}-viewers:");
        }

        public async Task AddViewerAsync(int idMatch)
        {
            await _redisDatabase.StringIncrementAsync($"{PREFIX_KEY}-{idMatch}-viewers:");
        }

        public async Task RemoveViewerAsync(int idMatch)
        {
            await _redisDatabase.StringDecrementAsync($"{PREFIX_KEY}-{idMatch}-viewers:");
        }
    }
}
