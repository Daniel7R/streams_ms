namespace StreamsMS.Application.Interfaces
{
    public interface IStreamViewerService
    {
        Task<bool> CanJoinStream(int matchId, int idUser);
        Task LeaveStreamAsync(int idMatch, int idUser);
    }
}
