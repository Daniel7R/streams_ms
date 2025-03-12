using StreamsMS.Domain.Entities;

namespace StreamsMS.Infrastructure.Repository
{
    public interface IStreamRepository: ICreateUpdateRepository<Streams>, IGetRepository<Streams>
    {
        Task ChangeUrlStream(int id, Uri url);
    }
}
