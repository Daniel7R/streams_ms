using StreamsMS.Application.DTOs.Request;
using StreamsMS.Application.Interfaces;
using StreamsMS.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace StreamsMS.Application.Services
{
    public class StreamsService : IStreamsService
    {
        
        
        public Task<bool> ChangeUrlStream(Uri streamsChangeUrlStream)
        {
            throw new NotImplementedException();
        }

        public Task<Streams> CreateStream(CreateStreamRequest streamsCreate)
        {
            throw new NotImplementedException();
        }

        public Task<Streams> GetStreamByIdMatch(int idMatch)
        {
            throw new NotImplementedException();
        }

        public async Task JoinStreamParticipant(int idMatch, int userId)
        {

            //VALIDATE TOURNAMENT IS FREE
            //validate strream byt match id
            // Upgrade REQUEST MAX VIEWERS MATCH

        }
    }
}
