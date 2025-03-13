using StackExchange.Redis;
using StreamsMS.Application.Interfaces;
using StreamsMS.Infrastructure.Services;

namespace StreamsMS.Application.Services
{
    public class StreamViewerService: IStreamViewerService
    {
        private readonly RedisViewerService _redisViewerService;

        private const string PREFIX_KEY = "stream";
        private const int LIMIT_FREE_VIEWERS = 2; //ONLY 2 FOR TESTING, COULD BE MORE
        public StreamViewerService(RedisViewerService redisViewer)
        {
            _redisViewerService = redisViewer;
        }

        public async  Task<bool> CanJoinStream(int matchId, int idUser, bool hasLimit)
        {
            //VALIDATIONS USER
            //VALIDAR TORNEO GRATIS Y LIMITE
            int currentViewers = await _redisViewerService.GetViewerCountAsync(matchId);

            if (hasLimit==true && currentViewers >= LIMIT_FREE_VIEWERS) return false;
            
            await _redisViewerService.AddViewerAsync(matchId);

            return true;
        }

        public async Task LeaveStreamAsync(int idMatch, int idUser) 
        {
            await _redisViewerService.RemoveViewerAsync(idMatch);
        }
    }
}
