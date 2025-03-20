

using System.Text;
using Microsoft.AspNetCore.SignalR;
using Moq;
using StreamsMS.API.Hubs;
using StreamsMS.Application.DTOs.Request;
using StreamsMS.Application.Interfaces;
using StreamsMS.Application.Messages;
using StreamsMS.Application.Services;
using StreamsMS.Domain.Entities;
using StreamsMS.Domain.Exceptions;
using StreamsMS.Infrastructure.Http;
using StreamsMS.Infrastructure.Repository;
using StreamsMS.Infrastructure.SignalR;
using StreamsMS.Tests.Fake;
using Xunit;

namespace StreamsMS.Tests
{

    public class StreamsServiceTests
    {
        private readonly Mock<IStreamRepository> _streamRepoMock;
        private readonly Mock<IPlatformRepository> _platformRepoMock;
        private readonly Mock<IEventBusProducer> _eventProducerMock;
        private readonly Mock<IHubContext<StreamHub>> _hubContextMock;
        private readonly Mock<IStreamViewerService> _streamViewerServiceMock;
        private readonly Mock<StreamConnectionManager> _connectionManagerMock;
        private readonly TicketHttpClient _ticketHttpClientMock;
        private readonly StreamsService _streamsService;

        public StreamsServiceTests()
        {
            _streamRepoMock = new Mock<IStreamRepository>();
            _platformRepoMock = new Mock<IPlatformRepository>();
            _eventProducerMock = new Mock<IEventBusProducer>();
            _hubContextMock = new Mock<IHubContext<StreamHub>>();
            _streamViewerServiceMock = new Mock<IStreamViewerService>();
            _connectionManagerMock = new Mock<StreamConnectionManager>();

            var fakeHandler = new FakeHttpMessageHandler(request =>
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{\"message\": \"Success\"}", Encoding.UTF8, "application/json")
            };
        });

            var fakeHttpClient = new HttpClient(fakeHandler) { BaseAddress = new Uri("https://fakeapi.com") };

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x["APIKEY:TICKETS"]).Returns("fake-api-key");

            _ticketHttpClientMock = new TicketHttpClient(fakeHttpClient, configMock.Object);


            _streamsService = new StreamsService(
                _eventProducerMock.Object,
                _streamViewerServiceMock.Object,
                _streamRepoMock.Object,
                _connectionManagerMock.Object,
                _hubContextMock.Object,
                _platformRepoMock.Object,
                _ticketHttpClientMock
            );
        }

        [Fact]
        public async Task ChangeUrlStream_WhenStreamDoesNotExist_ShouldThrowBusinessRuleException()
        {
            //arrange
            int idStream = 1, idUser = 10;
            var request = new ChangeUrlRequest { NewUrl = new Uri("https://newstream.com") };

            _streamRepoMock.Setup(repo => repo.GetById(idStream))
                .ReturnsAsync((Streams)null);

            //act & assert
            await Assert.ThrowsAsync<BusinessRuleException>(
                () => _streamsService.ChangeUrlStream(request, idStream, idUser)
            );
        }

        [Fact]
        public async Task ChangeUrlStream_WhenUserHasNoPermission_ShouldThrowInvalidRoleException()
        {
            //arrange
            int idStream = 1, idUser = 10;
            var request = new ChangeUrlRequest { NewUrl = new Uri("https://newstream.com") };
            var existingStream = new Streams { Id = idStream, IdMatch = 10 };

            _streamRepoMock.Setup(repo => repo.GetById(idStream))
                .ReturnsAsync(existingStream);

            _eventProducerMock.Setup(ep => ep.SendRequest<ValidateMatchRoleUser, ValidateMatchRoleUserResponse>(
                    It.IsAny<ValidateMatchRoleUser>(), It.IsAny<string>()
                ))
                .ReturnsAsync(new ValidateMatchRoleUserResponse { IsValidRoleUser = false, IsExistingMatch = true });

            //act & assert
            await Assert.ThrowsAsync<InvalidRoleException>(
                () => _streamsService.ChangeUrlStream(request, idStream, idUser)
            );
        }

        [Fact]
        public async Task ChangeUrlStream_WhenValid_ShouldChangeUrlAndReturnTrue()
        {
            //arrange
            int idStream = 1, idUser = 10;
            var request = new ChangeUrlRequest { NewUrl = new Uri("https://newstream.com") };
            var existingStream = new Streams { Id = idStream, IdMatch = 10 };

            _streamRepoMock.Setup(repo => repo.GetById(idStream))
                .ReturnsAsync(existingStream);

            _eventProducerMock.Setup(ep => ep.SendRequest<ValidateMatchRoleUser, ValidateMatchRoleUserResponse>(
                    It.IsAny<ValidateMatchRoleUser>(), It.IsAny<string>()
                ))
                .ReturnsAsync(new ValidateMatchRoleUserResponse { IsValidRoleUser = true, IsExistingMatch = true });

            _streamRepoMock.Setup(repo => repo.ChangeUrlStream(idStream, request.NewUrl))
                .Returns(Task.CompletedTask);

            //act
            var result = await _streamsService.ChangeUrlStream(request, idStream, idUser);

            //assert
            Assert.True(result);
        }

        [Fact]
        public async Task ChangeUrlStream_WhenUrlIsInvalid_ShouldThrowFormatException()
        {
            //arrange
            int idStream = 1, idUser = 10;
            var request = new ChangeUrlRequest { NewUrl = new Uri("invalid-url", UriKind.RelativeOrAbsolute) };

            //act & assert
            await Assert.ThrowsAsync<BusinessRuleException>(
                () => _streamsService.ChangeUrlStream(request, idStream, idUser)
            );
        }


        [Fact]
        public async Task ChangeUrlStream_WhenPlatformNotFound_ShouldThrowNotFoundException()
        {
            // arrange
            int idStream = 0, idUser = 10;
            var request = new ChangeUrlRequest { NewUrl = new Uri("https://newstream.com") };
            var existingStream = new Streams { Id = idStream, IdMatch = 100 };

            _streamRepoMock.Setup(repo => repo.GetById(idStream))
                .ReturnsAsync(existingStream);

            _platformRepoMock.Setup(repo => repo.GetById(existingStream.IdMatch))
                .ReturnsAsync((Platforms)null);

            // act & assert
            await Assert.ThrowsAsync<BusinessRuleException>(
                () => _streamsService.ChangeUrlStream(request, idStream, idUser)
            );
        }
    }
}