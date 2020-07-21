using ContactlessEntry.Cloud.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ContactlessEntry.Cloud.UnitTests.Services
{
    public class OpenDoorServiceTests
    {
        [Fact]
        public async Task OpenDoorAsync_WithHappyPath_CompletesSuccessfully()
        {
            var mockService = new Mock<IOpenDoorService>();
            mockService.Setup(s => s.OpenDoorAsync(It.IsNotNull<string>(), It.IsNotNull<string>()));

            IOpenDoorService openDoorService = mockService.Object;
            await openDoorService.OpenDoorAsync($"{Guid.NewGuid()}", $"{Guid.NewGuid()}");
        }
    }
}
