using Microsoft.AspNetCore.SignalR;
using StreamsMS.Application.Interfaces;
using StreamsMS.Infrastructure.SignalR;

namespace StreamsMS.API.Hubs
{
    public class StreamHub: Hub
    {
        private readonly IStreamViewerService _streamViewerService;
        private readonly StreamConnectionManager _connectionManger;

        public StreamHub(IStreamViewerService streamViewerService, StreamConnectionManager streamConnectionManager)
        {
            _streamViewerService = streamViewerService;
            _connectionManger = streamConnectionManager;
        }

        public async Task JoinStream(int matchId, int userId)
        {
            string connectionId = Context.ConnectionId;

            _connectionManger.AddConnection(connectionId, matchId, userId);

            bool allowed = await _streamViewerService.CanJoinStream(matchId, userId,true);

            if (!allowed)
            {
                await Clients.Caller.SendAsync("LimitReached", "Stream is full");
                Context.Abort(); //Disconnect user if limit is reached
            }
        }

        public async Task KickUser(int matchId, int userId)
        {
            // get user info
            string connectionId = _connectionManger.GetConnectionIdByUser(matchId, userId);

            if (connectionId != null)
            {
                // send message to ban
                await Clients.Client(connectionId).SendAsync("Kicked", "You have been removed from the stream");

                // delete system connection
                _connectionManger.RemoveConnection(connectionId);

                // Remove from viewers
                await _streamViewerService.LeaveStreamAsync(matchId, userId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            string connectionId = Context.ConnectionId;

            var connectionInfo = _connectionManger.GetConnectionInfo(connectionId);

            if(connectionInfo.HasValue)
            {
                int matchId = connectionInfo.Value.matchId;
                int userId = connectionInfo.Value.userId;

                await _streamViewerService.LeaveStreamAsync(matchId, userId);
                _connectionManger.RemoveConnection(connectionId);
            }
        }
    }
}
