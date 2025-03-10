using StreamsMS.Application.DTOs.Request;
using StreamsMS.Domain.Entities;

namespace StreamsMS.Application.Interfaces
{
    public interface IStreamsService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamsCreate"></param>
        /// <returns>Streams</returns>
        public Task<Streams> CreateStream(CreateStreamRequest streamsCreate);
        /// <summary>
        /// Gets the url for stream match
        /// </summary>
        /// <param name="idMatch"></param>
        /// <returns></returns>
        public Task<Streams> GetStreamByIdMatch(int idMatch);
        /// <summary>
        /// Change the url stream
        /// </summary>
        /// <param name="streamsChangeUrlStream"></param>
        /// <returns></returns>
        public Task<bool> ChangeUrlStream(Uri streamsChangeUrlStream);
    }
}
