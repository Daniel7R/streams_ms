using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StreamsMS.API.Hubs;
using StreamsMS.Application.DTOs.Request;
using StreamsMS.Application.DTOs.Response;
using StreamsMS.Application.Interfaces;
using StreamsMS.Domain.Entities;
using StreamsMS.Domain.Enums;
using StreamsMS.Domain.Exceptions;
using StreamsMS.Infrastructure.Services;
using StreamsMS.Infrastructure.SignalR;
using System.Security.Claims;

namespace StreamsMS.API.Controllers
{
    [Authorize]
    [Route("api/v1/streams")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class StreamsController : ControllerBase
    {
        private readonly StreamConnectionManager _connectionManager;
        private readonly IStreamViewerService _streamViewerService;
        private readonly RedisViewerService _redisViewerService;
        private readonly IHubContext<StreamHub> _hubContext;
        private readonly IStreamsService _streamService;

        public StreamsController(StreamConnectionManager streamConnectionManager, IStreamsService streamsService,IStreamViewerService streamViewerService, RedisViewerService redisViewerService, IHubContext<StreamHub> hubContext)
        {
            _connectionManager = streamConnectionManager;
            _streamViewerService = streamViewerService;
            _redisViewerService = redisViewerService;
            _hubContext= hubContext;
            _streamService = streamsService;
        }

        /// <summary>
        ///    Creates a stream for a valid match, before creating stream, it validates platform, user role for tournament and some other validations 
        /// </summary>
        /// <param name="streamsCreate"></param>
        /// <returns>Streams</returns>
        [HttpPost]
        [Route("", Name ="CreateStreamForMatch")]
        public async Task<IActionResult> CreateStreamForMatch([FromBody] CreateStreamRequest createStream)
        {
            var response = new ResponseDTO<StreamResponseDTO?>();
            try
            {
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);

                var stream = await _streamService.CreateStream(createStream,idUser);
                response.Result = stream;
                response.Message = "Successfully created";
                return Ok(response);
            } catch (BusinessRuleException ru)
            {
                response.Message = ru.Message;
                return BadRequest(response);
            } catch(InvalidRoleException ir)
            {
                response.Message = ir.Message;
                return Unauthorized(response);
            } catch (Exception ex)
            {
                response.Message=ex.Message;
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// This method changes the url for a stream
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{streamId}/url", Name ="ChangeUriStream")]
        public async Task<IActionResult> ChangeUrlStream(int streamId, [FromBody] ChangeUrlRequest request)
        {
            var response = new ResponseDTO<bool?>();
            try
            {
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);
                var status = await _streamService.ChangeUrlStream(request, streamId, idUser);
                response.Message="Url successfully changed";
                return Ok(response);
            }
            catch (BusinessRuleException ru)
            {
                response.Message = ru.Message;
                return BadRequest(response);
            }
            catch (InvalidRoleException ir)
            {
                response.Message = ir.Message;
                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// This method allows a valid user join to a match stream
        /// </summary>
        /// <param name="idStreamMatch"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        [Route("viewer/join", Name ="ViewerJoin")]
        public async Task<IActionResult> JoinStreamViewer([FromBody] UseTicketRequest request)
        {

            ResponseDTO<bool?> response = new ResponseDTO<bool?>();
            //validar si el usuario se puede unir
            //var canJoin = await _streamViewerService.CanJoinStream(1, 123);
            try
            {
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);
                request.IdUser= idUser;
                await _streamService.JoinStream(request, Roles.VIEWER);

                response.Message = "Successfully joined";
                return Ok(response);
            } catch(BusinessRuleException br)
            {
                response.Message = br.Message;
                return BadRequest(response);
            } catch(HttpRequestException he)
            {
                response.Message = he.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Message = "An unexpected error occurred";
                return StatusCode(500, response); 
            }

        }

        /// <summary>
        /// This method allows a valid user to join a tournament's match
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("participant/join", Name = "ParticipantJoin")]
        public async Task<IActionResult> JoinStreamParticipant([FromBody]UseTicketRequest request)
        {
            ResponseDTO<bool?> response = new ResponseDTO<bool?>();
            try
            {
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);
                request.IdUser = idUser;
                await _streamService.JoinStream(request, Roles.PARTICIPANT);
                
                response.Message= "Successfully joined";
                return Ok(response);
            } catch(BusinessRuleException br)
            {
                response.Message = br.Message;
                return BadRequest(response);
            } catch(Exception ex)
            {
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Get the viewers count on a stream
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{matchId}/viewers", Name ="GetViewersCount")]
        [ProducesResponseType(StatusCodes.Status200OK, Type= typeof(ResponseDTO<ViewersCountDTO?>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type= typeof(ResponseDTO<ViewersCountDTO?>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type= typeof(ResponseDTO<ViewersCountDTO?>))]
        public async Task<IActionResult> GetViewerCount(int matchId)
        {
            var response = new ResponseDTO<ViewersCountDTO>();
            try{
                
                var counterMatch= await _streamViewerService.GetViewersByMatch(matchId);
                response.Result= counterMatch;
                response.Message= "Successfully requested";
                
                return Ok(response);
            } catch (BusinessRuleException br){
                response.Message= br.Message;

                return BadRequest(response);
            } catch(Exception ex){
                response.Message= ex.Message;

                return StatusCode(500,response);
            }
        }


        /// <summary>
        /// Remove user from stream(block to user and it would be through a signalR request)
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("kick", Name ="KickUser")]
        public async Task<IActionResult> KickUser([FromBody]KickUserRequest kickUser)
        {
            var responseDTO = new ResponseDTO<string?>();
            try
            {
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);

                await _streamService.KickUser(kickUser, idUser);
                return Ok(new { message = "User removed from stream" });
            } catch(BusinessRuleException br)
            {
                responseDTO.Message = br.Message;
                return BadRequest(responseDTO);
            } catch(InvalidRoleException ir)
            {
                responseDTO.Message = ir.Message;
                return BadRequest(responseDTO);
            } catch(Exception ex)
            {
                responseDTO.Message = ex.Message;
                return Conflict(responseDTO);
            }

        }

        private string? ExtractUserId()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

            return userId;
        }
    }
}
