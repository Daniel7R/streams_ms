using System.ComponentModel.DataAnnotations;

namespace StreamsMS.Domain.Entities
{
    public class Platforms
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Streams> Streams { get; set; }
    }
}
