using ContactlessEntry.Cloud.Configuration;
using ContactlessEntry.Cloud.Models;
using ContactlessEntry.Cloud.Services;
using Divergic.Logging.Xunit;
using FluentAssertions;
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
            access.Should().NotBeNull();
            access.DoorId.Should().NotBeNullOrEmpty();
            access.Granted.Should().BeTrue();
            access.PersonId.Should().NotBeNullOrWhiteSpace();
            access.Temperature.Should().BeGreaterThan(0);
            access.Timestamp.Should().BeAfter(DateTime.MinValue);
        }

        [Fact]
        public async Task RequestAccessAsync_WithoutDoorId_ThrowsArgumentNullException()
        {
            IAccessManager accessManager = new AccessManager(Mock.Of<ILogger<AccessManager>>(), Mock.Of<IOpenDoorService>(), Mock.Of<IAccessRepository>(), Mock.Of<IMicroserviceSettings>());
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await accessManager.RequestAccessAsync(null, $"{Guid.NewGuid()}", 0);
            });
        }

        [Fact]
        public async Task RequestAccessAsync_WithoutPersonId_ThrowsArgumentNullException()
        {
            IAccessManager accessManager = new AccessManager(Mock.Of<ILogger<AccessManager>>(), Mock.Of<IOpenDoorService>(), Mock.Of<IAccessRepository>(), Mock.Of<IMicroserviceSettings>());
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await accessManager.RequestAccessAsync($"{Guid.NewGuid()}", null, 0);
            });
        }

        [Fact]
        public async Task RequestAccessAsync_WithHighTemperature_DeniesAccess()
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

            var access = await accessManager.RequestAccessAsync($"{Guid.NewGuid()}", $"{Guid.NewGuid()}", 38);
            access.Should().NotBeNull();
            access.DoorId.Should().NotBeNullOrEmpty();
            access.Granted.Should().BeFalse();
            access.PersonId.Should().NotBeNullOrWhiteSpace();
            access.Temperature.Should().BeGreaterThan(0);
            access.Timestamp.Should().BeAfter(DateTime.MinValue);
        }

        [Fact]
        public async Task RequestAccessAsync_WithExceptionInRepository_ThrowsException()
        {
            var logger = _loggerFactory.CreateLogger<AccessManager>();
            var mockOpenDoor = Mock
                .Of<IOpenDoorService>();

            var mockAccessRepository = new Mock<IAccessRepository>();
            mockAccessRepository
                .Setup(ar => ar.CreateAccessAsync(It.IsNotNull<Access>()))
                .Throws<NotImplementedException>();

            var mockMicroserviceSettings = Mock
                .Of<MicroserviceSettings>(ms => ms.MaxAllowedTemperature == 37.9);

            IAccessManager accessManager = new AccessManager(logger, mockOpenDoor, mockAccessRepository.Object, mockMicroserviceSettings);
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                await accessManager.RequestAccessAsync($"{Guid.NewGuid()}", $"{Guid.NewGuid()}", 38);
            });
            
        }
    }
}
