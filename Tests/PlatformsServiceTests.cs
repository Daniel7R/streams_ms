

using Moq;
using StreamsMS.Application.Interfaces;
using StreamsMS.Application.Services;
using StreamsMS.Domain.Entities;
using StreamsMS.Infrastructure.Repository;
using Xunit;

namespace StreamsMS.Tests{
    public class PlatformServiceTests{
        private readonly Mock<IPlatformRepository> _platformRepoMock;
        private readonly IPlatformsService _platformsServiceMock;

        public PlatformServiceTests(){
            _platformRepoMock = new Mock<IPlatformRepository>();
            _platformsServiceMock = new PlatformsService(_platformRepoMock.Object);
        }

        [Fact]
        public async Task GetPlatforms_Should_Returin_PlatformDTOLIst(){
            //arrage
            var platforms = new List<Platforms>{
                new Platforms{Id=1, Name= "Twithc"},
                new Platforms{Id=2, Name= "YouTube"}
            };

            _platformRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(platforms);

            //act
            var result = await _platformsServiceMock.GetPlatforms();

            //assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Collection(result,
                item => Assert.Equal("Twithc", item.NamePlatform),
                item => Assert.Equal("YouTube", item.NamePlatform)
            );
        }

        [Fact]
        public async Task GetPlatforms_Should_Return_EmptyList_When_No_Platforms(){
            //arrange
            _platformRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(new List<Platforms>());

            //act
            var result =await _platformsServiceMock.GetPlatforms();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

    }
}