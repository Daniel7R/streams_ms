using StreamsMS.Application.DTOs.Response;

namespace StreamsMS.Application.Interfaces
{
    public interface IStreamViewerService
    {
        Task<bool> CanJoinStream(int matchId, int idUser, bool hasLimit);
        Task LeaveStreamAsync(int idMatch, int idUser);
        Task<ViewersCountDTO> GetViewersByMatch(int idMatch);
    }
}
