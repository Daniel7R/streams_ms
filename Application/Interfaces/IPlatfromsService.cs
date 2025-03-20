using StreamsMS.Application.DTOs.Response;
using StreamsMS.Domain.Entities;

namespace StreamsMS.Application.Interfaces{
    public interface IPlatformsService{
        Task<IEnumerable<PlatformDTO>>  GetPlatforms();
    }
}