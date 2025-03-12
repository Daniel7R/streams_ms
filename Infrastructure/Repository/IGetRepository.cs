using StreamsMS.Domain.Entities;

namespace StreamsMS.Infrastructure.Repository
{
    public interface IGetRepository<T> where T : class
    {
        Task<T> GetById(int id);
    }
}
