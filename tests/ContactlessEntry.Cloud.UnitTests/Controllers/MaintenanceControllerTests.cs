using ContactlessEntry.Cloud.Controllers;
using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.Services;
using FluentAssertions;
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
                .Returns(() => Task.FromResult(new Cloud.Models.Person { PersonId = $"{Guid.NewGuid()}" }));

            var dto = new NewPersonDto
            {
                FaceUrl = $"http://www.google.com",
                Name = $"{Guid.NewGuid()}"
            };

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            var actionResult = await controller.AddPersonAsync(dto);
            Assert.NotNull(actionResult);

            var okObjectResult = Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            var person = Assert.IsAssignableFrom<Cloud.Models.Person>(okObjectResult.Value);
            person.Should().NotBeNull();
            person.PersonId.Should().NotBeNullOrWhiteSpace();
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

        [Fact]
        public async Task RemovePersonAsync_WithMissingInput_ReturnsBadRequest()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = Mock.Of<IFaceClientService>();

            var controller = new MaintenanceController(mockFaceClientService, mockLogger);
            var actionResult = await controller.RemovePersonAsync(null);
            Assert.NotNull(actionResult);
            Assert.IsAssignableFrom<BadRequestResult>(actionResult);
        }

        [Fact]
        public async Task RemovePersonAsync_WithCorrectInput_ReturnsOk()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.DeletePersonAsync(It.IsNotNull<string>()));

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            var actionResult = await controller.RemovePersonAsync($"{Guid.NewGuid()}");
            Assert.NotNull(actionResult);

            Assert.IsAssignableFrom<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task RemovePersonAsync_WithFaceClientServiceFault_ThrowsException()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.DeletePersonAsync(It.IsNotNull<string>()))
                .Throws<NotImplementedException>();

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                await controller.RemovePersonAsync($"{Guid.NewGuid()}");
            });
        }

        [Fact]
        public async Task AddFaceAsync_WithMissingPersonId_ReturnsBadRequest()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = Mock.Of<IFaceClientService>();

            var controller = new MaintenanceController(mockFaceClientService, mockLogger);
            var actionResult = await controller.AddFaceAsync(null, new FaceDto());
            Assert.NotNull(actionResult);
            Assert.IsAssignableFrom<BadRequestResult>(actionResult);
        }

        [Fact]
        public async Task AddFaceAsync_WithMissingDto_ReturnsBadRequest()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = Mock.Of<IFaceClientService>();

            var controller = new MaintenanceController(mockFaceClientService, mockLogger);
            var actionResult = await controller.AddFaceAsync($"{Guid.NewGuid()}", default);
            Assert.NotNull(actionResult);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task AddFaceAsync_WithCorrectInput_ReturnsOk()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.AddFaceAsync(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Returns(() => Task.FromResult(new Cloud.Models.Face { FaceId = $"{Guid.NewGuid()}" }));

            var dto = new FaceDto
            {
                FaceUrl = $"http://www.google.com"
            };

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            var actionResult = await controller.AddFaceAsync($"{Guid.NewGuid()}", dto);
            Assert.NotNull(actionResult);

            var okObjectResult = Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            var face = Assert.IsAssignableFrom<Cloud.Models.Face>(okObjectResult.Value);
            face.Should().NotBeNull();
            face.FaceId.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task AddFaceAsync_WithFaceClientServiceFault_ThrowsException()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.AddFaceAsync(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Throws<NotImplementedException>();

            var dto = new FaceDto
            {
                FaceUrl = $"http://www.google.com"
            };

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                await controller.AddFaceAsync($"{Guid.NewGuid()}", dto);
            });
        }

        [Fact]
        public async Task RemoveFaceAsync_WithMissingPersonId_ReturnsBadRequest()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = Mock.Of<IFaceClientService>();

            var controller = new MaintenanceController(mockFaceClientService, mockLogger);
            var actionResult = await controller.RemoveFaceAsync(null, $"{Guid.NewGuid()}");
            Assert.NotNull(actionResult);
            Assert.IsAssignableFrom<BadRequestResult>(actionResult);
        }

        [Fact]
        public async Task RemoveFaceAsync_WithMissingFaceId_ReturnsBadRequest()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = Mock.Of<IFaceClientService>();

            var controller = new MaintenanceController(mockFaceClientService, mockLogger);
            var actionResult = await controller.RemoveFaceAsync($"{Guid.NewGuid()}", null);
            Assert.NotNull(actionResult);
            Assert.IsAssignableFrom<BadRequestResult>(actionResult);
        }

        [Fact]
        public async Task RemoveFaceAsync_WithCorrectInput_ReturnsOk()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.RemoveFaceAsync(It.IsNotNull<string>(), It.IsNotNull<string>()));

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            var actionResult = await controller.RemoveFaceAsync($"{Guid.NewGuid()}", $"{Guid.NewGuid()}");
            Assert.NotNull(actionResult);

            Assert.IsAssignableFrom<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task RemoveFaceAsync_WithFaceClientServiceFault_ThrowsException()
        {
            var mockLogger = Mock.Of<ILogger<MaintenanceController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.RemoveFaceAsync(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Throws<NotImplementedException>();

            var controller = new MaintenanceController(mockFaceClientService.Object, mockLogger);
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                await controller.RemoveFaceAsync($"{Guid.NewGuid()}", $"{Guid.NewGuid()}");
            });
        }
    }
}
