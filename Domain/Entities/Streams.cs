using System.Text.Json.Serialization;

namespace StreamsMS.Domain.Entities
{
    public class Streams
    {
        public int Id { get; set; }
        public int IdPlatform {  get; set; }
        [JsonIgnore]
        public Platforms Platform { get; set; }
        public Uri UrlStream { get; set; }
        public int IdMatch {  get; set; } 
    }
}
