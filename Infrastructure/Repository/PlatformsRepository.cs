using Microsoft.EntityFrameworkCore;
using StreamsMS.Domain.Entities;
using StreamsMS.Infrastructure.Data;

namespace StreamsMS.Infrastructure.Repository
{
    public class PlatformsRepository : IPlatformRepository
    {
        private readonly StreamsDbContext _context;

        public PlatformsRepository(StreamsDbContext context)
        {
            _context = context;
        }
        public async Task<Platforms> GetById(int id)
        {
            return await _context.Platforms.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Platforms>> GetAll(){
            return await _context.Platforms.ToListAsync();
        }
    }
}
