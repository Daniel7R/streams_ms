namespace StreamsMS.Infrastructure.Repository
{
    public interface ICreateUpdateRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
    }
}
