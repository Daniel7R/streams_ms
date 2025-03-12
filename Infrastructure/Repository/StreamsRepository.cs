using Microsoft.EntityFrameworkCore;
using StreamsMS.Domain.Entities;
using StreamsMS.Infrastructure.Data;

namespace StreamsMS.Infrastructure.Repository
{
    public class StreamsRepository : IStreamRepository
    {
        private readonly StreamsDbContext _context;
        private readonly DbSet<Streams> _streams;

        public StreamsRepository(StreamsDbContext context)
        {
            _context = context;
            _streams= context.Set<Streams>();
        }
        public async Task<Streams> AddAsync(Streams entity)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                _streams.Add(entity);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return entity;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public async Task ChangeUrlStream(int id, Uri url)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                var stream = await _context.Streams.FindAsync(id);
                _context.Attach(stream);
                stream.UrlStream = url;
                _context.Entry(stream).Property(s => s.UrlStream).IsModified = true;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public async Task<Streams> GetById(int id)
        {
            return await _context.Streams.Where(s => s.Id == id).FirstOrDefaultAsync();

        }

        public Task UpdateAsync(Streams entity)
        {
            throw new NotImplementedException();
        }
    }
}
