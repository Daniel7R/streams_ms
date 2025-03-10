namespace StreamsMS.Application.DTOs.Request
{
    public class CreateStreamRequest
    {
        public int IdPlatform { get; set; }
        public Uri StreamUrl { get; set; }
        public int IdMatch { get; set; }    
    }
}
