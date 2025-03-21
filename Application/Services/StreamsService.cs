using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using StreamsMS.API.Hubs;
using StreamsMS.Application.DTOs.Request;
using StreamsMS.Application.DTOs.Response;
using StreamsMS.Application.Interfaces;
using StreamsMS.Application.Messages;
using StreamsMS.Domain.Entities;
using StreamsMS.Domain.Enums;
using StreamsMS.Domain.Exceptions;
using StreamsMS.Infrastructure.Http;
using StreamsMS.Infrastructure.Repository;
using StreamsMS.Infrastructure.SignalR;
using System.Text.RegularExpressions;

namespace StreamsMS.Application.Services
{
    public class StreamsService : IStreamsService
    {
        private readonly IEventBusProducer _eventProducer;
        private readonly IStreamRepository _streamRepo;
        private readonly IPlatformRepository _platformRepo;
        private readonly IHubContext<StreamHub> _hubContext;
        private readonly StreamConnectionManager _connectionManager;
        private readonly IStreamViewerService _streamViewerService;
        private readonly TicketHttpClient _ticketHttpClient;
        public StreamsService(IEventBusProducer eventBusProducer, IStreamViewerService streamViewerService, IStreamRepository streamRepository, StreamConnectionManager connectionManager, IHubContext<StreamHub> hubContext, IPlatformRepository platformRepo, TicketHttpClient ticketHttpClient)
        {
            _ticketHttpClient = ticketHttpClient;
            _eventProducer = eventBusProducer;
            _streamRepo = streamRepository;
            _platformRepo = platformRepo;
            _hubContext = hubContext;
            _streamViewerService = streamViewerService;
            _connectionManager = connectionManager;
        }

        public async Task<bool> ChangeUrlStream(ChangeUrlRequest streamsChangeUrlStream, int idStream, int idUser)
        {
            ValidateUrlWithRegex(streamsChangeUrlStream.NewUrl.ToString());
            var stream = await _streamRepo.GetById(idStream);
            if (stream == null || stream.Id == 0) throw new BusinessRuleException("Stream does not exist");

            await ValidateMatchRole(idUser, stream.IdMatch);

            await _streamRepo.ChangeUrlStream(idStream, streamsChangeUrlStream.NewUrl);

            return true;
        }

        public async Task<StreamResponseDTO> CreateStream(CreateStreamRequest streamsCreate, int idUser)
        {
            //validate uri
            ValidateUrlWithRegex(streamsCreate.StreamUrl.ToString());
            //VALIDAR PARTIDO Y ROL USER
        
            await ValidateMatchRole(idUser, streamsCreate.IdMatch);
            //validate platform
            var platform = await _platformRepo.GetById(streamsCreate.IdPlatform);
            if (platform == null || platform.Id == 0) throw new BusinessRuleException("Invalid platform provided");

            var stream = new Streams
            {
                IdMatch = streamsCreate.IdMatch,
                IdPlatform = platform.Id,
                Platform = platform,
                UrlStream = streamsCreate.StreamUrl
            };
            var created = await _streamRepo.AddAsync(stream);

            var streamResponse = new StreamResponseDTO
            {
                NamePlatform = platform.Name,
                Url = streamsCreate.StreamUrl.ToString(),
                IdStream = created.Id,
                IdMatch = stream.IdMatch,
            };

            return streamResponse;
        }


        public async Task JoinStream(UseTicketRequest request, Roles roleJoiner)
        {
            //VALIDATE TOURNAMENT IS FREE
            var isFreeMatchTournament = await _eventProducer.SendRequest<int, bool?>(request.IdMatch, Queues.Queues.IS_FREE_MATCH_TOURNAMENT);

            if (isFreeMatchTournament == null) throw new BusinessRuleException("Match provided could be null");
                    string? id = _connectionManager.GetConnectionIdByUser(request.IdMatch, request.IdUser);

            if (isFreeMatchTournament == true && roleJoiner.Equals(Roles.VIEWER))
            {
                //validates if user can join stream, if can join, it would add user
                bool canJoin = await _streamViewerService.CanJoinStream(request.IdMatch, request.IdUser, true);
                if (!canJoin)
                {
                    if (id != null)
                        _connectionManager.RemoveConnection(id);
                    throw new BusinessRuleException("Viewers have reached the free limit");
                }

                _connectionManager.AddConnection(id, request.IdMatch, request.IdUser);
            }
            else
            {
                var requestUseParticipant = await _ticketHttpClient.UseTicket(request);
                await _streamViewerService.CanJoinStream(request.IdMatch, request.IdUser, false);
                if(id != null)
                    _connectionManager.AddConnection(id, request.IdMatch, request.IdUser);
            }


        }

        private async Task ValidateMatchRole(int idUser, int idMatch)
        {
            //VALIDAR PARTIDO Y ROL USER
            var request = new ValidateMatchRoleUser
            {
                IdMatch = idMatch,
                IdUser = idUser
            };
            var validation = await _eventProducer.SendRequest<ValidateMatchRoleUser, ValidateMatchRoleUserResponse>(request, Queues.Queues.VALIDATE_MATCH_AND_ROLE);

            if (validation != null && !validation.IsExistingMatch)
            {
                throw new BusinessRuleException("Match does not exist");
            }
            if (validation != null && !validation.IsValidRoleUser)
            {
                throw new InvalidRoleException("User has no permissions");
            }

        }

        public async Task KickUser(KickUserRequest kickUser, int idKicker)
        {
            await ValidateMatchRole(idKicker, kickUser.IdMatch);

            var connectionId = _connectionManager.GetConnectionIdByUser(kickUser.IdMatch, kickUser.IdUser2Kick);
            if (connectionId == null)
                throw new BusinessRuleException("User is not in the stream");

            await _hubContext.Clients.Client(connectionId).SendAsync("Kicked", "You have been removed from the stream");

            // delete user connection
            _connectionManager.RemoveConnection(connectionId);

            //remove fro stream
            await _streamViewerService.LeaveStreamAsync(kickUser.IdMatch, kickUser.IdUser2Kick);
        }

        private void ValidateUrlWithRegex(string url)
        {
            var urlRegex = new Regex(
                @"^(https?|ftps?|http?):\/\/(?:[a-zA-Z0-9]" +
                        @"(?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}" +
                        @"(?::(?:0|[1-9]\d{0,3}|[1-5]\d{4}|6[0-4]\d{3}" +
                        @"|65[0-4]\d{2}|655[0-2]\d|6553[0-5]))?" +
                        @"(?:\/(?:[-a-zA-Z0-9@%_\+.~#?&=]+\/?)*)?$",
                RegexOptions.IgnoreCase);

            urlRegex.Matches(url);

            if (!urlRegex.IsMatch(url)) throw new BusinessRuleException("Not valid URI");
        }
    }
}
