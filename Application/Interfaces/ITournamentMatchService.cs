namespace StreamsMS.Application.Interfaces
{
    public interface ITournamentMatchService
    {
        Task ValidateMatch();
        Task ValidateTournament();
    }
}
