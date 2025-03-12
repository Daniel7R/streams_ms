using StreamsMS.Application.DTOs.Request;
using StreamsMS.Application.DTOs.Response;
using StreamsMS.Domain.Entities;
using StreamsMS.Domain.Enums;

namespace StreamsMS.Application.Interfaces
{
    public interface IStreamsService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamsCreate"></param>
        /// <returns>Streams</returns>
        public Task<StreamResponseDTO> CreateStream(CreateStreamRequest streamsCreate, int idUser);
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
        public Task<bool> ChangeUrlStream(ChangeUrlRequest streamsChangeUrlStream,int idStream, int idUser);

        /// <summary>
        /// Kicks a user from  stream
        /// </summary>
        /// <param name="kickUser"></param>
        /// <param name="idKicker"></param>
        /// <returns></returns>
        Task KickUser(KickUserRequest kickUser, int idKicker);
        /// <summary>
        ///  Joins an user to stream, whether it is free viewer, or paid, or even participant
        /// </summary>
        /// <param name="request"></param>
        /// <param name="roleJoiner"></param>
        /// <returns></returns>
        Task JoinStream(UseTicketRequest request, Roles roleJoiner);
    }
}
