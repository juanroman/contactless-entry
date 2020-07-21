using ContactlessEntry.Cloud.Configuration;
using ContactlessEntry.Cloud.Models;
using ContactlessEntry.Cloud.Services;
using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Services
{
    public class AccessManagerTests
    {
        private readonly ILoggerFactory _loggerFactory;

        public AccessManagerTests(ITestOutputHelper outputHelper)
        {
            _loggerFactory = LogFactory.Create(outputHelper);
            _loggerFactory.AddProvider(new TestOutputLoggerProvider(outputHelper));
        }

        [Fact]
        public async Task RequestAccessAsync_WithHappyPath_ReturnsAccess()
        {
            var logger = _loggerFactory.CreateLogger<AccessManager>();
            var mockOpenDoor = Mock
                .Of<IOpenDoorService>();

            var mockAccessRepository = new Mock<IAccessRepository>();
            mockAccessRepository
                .Setup(ar => ar.CreateAccessAsync(It.IsNotNull<Access>()))
                .Returns((Access input) => Task.FromResult(input));

            var mockMicroserviceSettings = Mock
                .Of<MicroserviceSettings>(ms => ms.MaxAllowedTemperature == 37.9);

            IAccessManager accessManager = new AccessManager(logger, mockOpenDoor, mockAccessRepository.Object, mockMicroserviceSettings);

            var access = await accessManager.RequestAccessAsync($"{Guid.NewGuid()}", $"{Guid.NewGuid()}", 37.1d);
            Assert.NotNull(access);
            Assert.True(access.Granted);
        }
    }
}
