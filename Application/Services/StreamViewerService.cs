using StackExchange.Redis;
using StreamsMS.Application.DTOs.Response;
using StreamsMS.Application.Interfaces;
using StreamsMS.Application.Messages;
using StreamsMS.Application.Queues;
using StreamsMS.Domain.Exceptions;
using StreamsMS.Infrastructure.Services;

namespace StreamsMS.Application.Services
{
    public class StreamViewerService: IStreamViewerService
    {
        private readonly RedisViewerService _redisViewerService;
        private readonly IEventBusProducer _eventProducer;

        private const string PREFIX_KEY = "stream";
        private const int LIMIT_FREE_VIEWERS = 2; //ONLY 2 FOR TESTING, COULD BE MORE
        public StreamViewerService(RedisViewerService redisViewer, IEventBusProducer eventBusProducer)
        {
            _redisViewerService = redisViewer;
            _eventProducer= eventBusProducer;
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

        public async Task<ViewersCountDTO> GetViewersByMatch(int idMatch){
            
            var requestValidateMatch =await _eventProducer.SendRequest<int, GetMatchByIdResponse>(idMatch, Queues.Queues.GET_MATCH_INFO);
            if(requestValidateMatch== null || requestValidateMatch.IdMatch == 0) throw new BusinessRuleException($"Provided match does not exist");
            
            var count = await _redisViewerService.GetViewerCountAsync(idMatch);
            var response = new ViewersCountDTO{
                MatchId= idMatch,
                Viewers = count
            };

            return response; 
        }
    }
}
