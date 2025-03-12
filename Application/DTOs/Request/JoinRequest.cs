namespace StreamsMS.Application.DTOs.Request
{
    public class JoinRequest
    {
        public int IdMatch { get; set; }
        //Only required when tournament is not free for participants
        public string? Code { get; set; }
    }
}
