using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StreamsMS.API.Hubs;
using StreamsMS.Application.DTOs.Request;
using StreamsMS.Application.Interfaces;
using StreamsMS.Infrastructure.Services;
using StreamsMS.Infrastructure.SignalR;

namespace StreamsMS.API.Controllers
{
    [Route("api/v1/streams")]
    [ApiController]
    public class StreamsController : ControllerBase
    {
        private readonly StreamConnectionManager _connectionManager;
        private readonly IStreamViewerService _streamViewerService;
        private readonly RedisViewerService _redisViewerService;
        private readonly IHubContext<StreamHub> _hubContext;

        public StreamsController(StreamConnectionManager streamConnectionManager, IStreamViewerService streamViewerService, RedisViewerService redisViewerService, IHubContext<StreamHub> hubContext)
        {
            _connectionManager = streamConnectionManager;
            _streamViewerService = streamViewerService;
            _redisViewerService = redisViewerService;
            _hubContext= hubContext;
        }
        [HttpGet]
        [Route("")]
        public Task<IActionResult> CreateStreamForMatch([FromBody] CreateStreamRequest createStream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method allows a valid user join to a match stream
        /// </summary>
        /// <param name="idStreamMatch"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet]
        [Route("viewer/join/{idStreamMatch}")]
        public Task<IActionResult> JoinStreamViewer([FromRoute] int idStreamMatch)
        {

            //validar si el usuario se puede unir
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> LeaveStream()
        {
            return Ok(new { message = "Acceso concedido al stream." });

        }

        [HttpGet("{matchId}/viewers")]
        public async Task<IActionResult> GetViewerCount(int matchId)
        {
            int count = await _redisViewerService.GetViewerCountAsync(matchId);
            return Ok(new { matchId, viewers = count });
        }


        /// <summary>
        /// Remove user from stream
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("{matchId}/kick/{userId}")]
        public async Task<IActionResult> KickUser(int matchId, int userId)
        {
            // Seerch user connection
            var connectionId = _connectionManager.GetConnectionIdByUser(matchId, userId);
            if (connectionId == null)
            {
                return NotFound(new { message = "User is not in the stream" });
            }

            await _hubContext.Clients.Client(connectionId).SendAsync("Kicked", "You have been removed from the stream");

            // delete user connection
            _connectionManager.RemoveConnection(connectionId);

            //remove fro stream
            await _streamViewerService.LeaveStreamAsync(matchId, userId);

            return Ok(new { message = "User removed from stream" });
        }


        /// <summary>
        /// This method allows a valid user to join a tournament's match
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("partcipant/join/{idMatch}")]
        public Task<IActionResult> JoinStreamParticipant(int idMatch)
        {
            throw new NotImplementedException();
        }
    }
}
