using System.Collections.Concurrent;

namespace StreamsMS.Infrastructure.SignalR
{
    public class StreamConnectionManager
    {
        private readonly ConcurrentDictionary<string, (int idMatch, int idUser)> _connections = new();

        public void AddConnection(string connectionId, int idMatch, int idUser)
        {
            _connections[connectionId]=(idMatch, idUser);
        }
        public (int matchId, int userId)? GetConnectionInfo(string connectionId)
        {
            return _connections.TryGetValue(connectionId, out var info) ? info : null;
        }

        public string GetConnectionIdByUser(int matchId, int userId)
        {
            return _connections.FirstOrDefault(x => x.Value.idMatch == matchId && x.Value.idUser == userId).Key;
        }

        public void RemoveConnection(string connectionId)
        {
            _connections.TryRemove(connectionId, out _);
        }
    }
}
