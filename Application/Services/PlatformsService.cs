using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using StreamsMS.API.Hubs;
using StreamsMS.Application.DTOs.Request;
using StreamsMS.Application.DTOs.Response;
using StreamsMS.Application.Interfaces;
using StreamsMS.Application.Messages;
using StreamsMS.Domain.Entities;
using StreamsMS.Domain.Enums;
using StreamsMS.Domain.Exceptions;
using StreamsMS.Infrastructure.Http;
using StreamsMS.Infrastructure.Repository;
using StreamsMS.Infrastructure.SignalR;
using System.Text.RegularExpressions;

namespace StreamsMS.Application.Services
{
    public class PlatformsService : IPlatformsService
    {
        private readonly IPlatformRepository  _platfromsRepo;
        public PlatformsService(IPlatformRepository platformRepository)
        {
            _platfromsRepo= platformRepository;
        }
        public async Task<IEnumerable<PlatformDTO>> GetPlatforms()
        {
            var platforms = await _platfromsRepo.GetAll();
            var convertedPlatforms = platforms.Select(x => new PlatformDTO{IdPlatform = x.Id, NamePlatform = x.Name}).ToList();

            return convertedPlatforms;
        }
    }
}
