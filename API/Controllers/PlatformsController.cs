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
    [Route("api/v1/platforms")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformsService _platformService;

        public PlatformsController(IPlatformsService platformsService)
        {
            _platformService= platformsService;
        }

        /// <summary>
        /// Get the available platforms for streams
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("", Name ="GetPlatforms")]
        public async Task<IActionResult> GetPlatforms()
        {
            var response = new ResponseDTO<IEnumerable<PlatformDTO>?>();
            try{
                var platforms= await _platformService.GetPlatforms();
                response.Result= platforms;
                response.Message= "Succedded";

                return Ok(response);
            }catch(Exception ex){
                response.Message= ex.Message;

                return StatusCode(500, response);
            }
        }
    }
}
