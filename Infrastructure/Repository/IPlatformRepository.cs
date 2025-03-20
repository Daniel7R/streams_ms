using StreamsMS.Domain.Entities;

namespace StreamsMS.Infrastructure.Repository
{
    public interface IPlatformRepository: IGetRepository<Platforms>
    {
        
        Task<Platforms> GetById(int id);
        Task<IEnumerable<Platforms>> GetAll();
    }
}
