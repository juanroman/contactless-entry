using ContactlessEntry.Cloud.Controllers;
using ContactlessEntry.Cloud.Models;
using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ContactlessEntry.Cloud.UnitTests.Controllers
{
    public class MaintenanceControllerTests
    {
        [Fact]
        public async Task AddPersonAsync_WithMissingInput_ReturnsBadRequest()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = Mock.Of<IFaceClientService>();

            var controller = new MaintenanceController(mockFaceClientService, mockLogger);
            var actionResult = await controller.AddPersonAsync(null);
            Assert.NotNull(actionResult);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task AddPersonAsync_WithCorrectInput_ReturnsOk()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.CreatePersonAsync(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Returns(() => Task.FromResult(new Models.Person { PersonId = $"{Guid.NewGuid()}" }));

            var dto = new NewPersonDto
            {
                FaceUrl = $"http://www.google.com",
                Name = $"{Guid.NewGuid()}"
            };

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            var actionResult = await controller.AddPersonAsync(dto);
            Assert.NotNull(actionResult);

            var okObjectResult = Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            Assert.IsAssignableFrom<Models.Person>(okObjectResult.Value);
        }

        [Fact]
        public async Task AddPersonAsync_WithFaceClientServiceFault_ThrowsException()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.CreatePersonAsync(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Throws<NotImplementedException>();

            var dto = new NewPersonDto
            {
                FaceUrl = $"http://www.google.com",
                Name = $"{Guid.NewGuid()}"
            };

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                await controller.AddPersonAsync(dto);
            });
        }

        [Fact]
        public async Task GetTrainingStatusAsync_WithCorrectInput_ReturnsOk()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.GetTrainingStatusAsync())
                .Returns(() => Task.FromResult(new TrainingStatus { Status = TrainingStatusType.Succeeded }));

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            var actionResult = await controller.GetTrainingStatusAsync();
            Assert.NotNull(actionResult);

            var okObjectResult = Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            Assert.IsAssignableFrom<TrainingStatus>(okObjectResult.Value);
        }

        [Fact]
        public async Task GetTrainingStatusAsync_WithFaceClientServiceFault_ThrowsException()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.GetTrainingStatusAsync())
                .Throws<NotImplementedException>();

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                await controller.GetTrainingStatusAsync();
            });
        }

        [Fact]
        public async Task BeginTrainingAsync_WithCorrectInput_ReturnsOk()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = Mock.Of<IFaceClientService>();

            var controller = new MaintenanceController(mockFaceClientService, mockLogger);
            var actionResult = await controller.BeginTrainingAsync();
            Assert.NotNull(actionResult);

            Assert.IsAssignableFrom<OkResult>(actionResult);
        }

        [Fact]
        public async Task BeginTrainingAsync_WithFaceClientServiceFault_ThrowsException()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.BeginTrainingAsync())
                .Throws<NotImplementedException>();

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                await controller.BeginTrainingAsync();
            });
        }
    }
}
