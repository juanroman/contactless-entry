using ContactlessEntry.Cloud.Controllers;
using ContactlessEntry.Cloud.Models;
using ContactlessEntry.Cloud.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ContactlessEntry.Cloud.UnitTests.Controllers
{
    public class FaceRecognitionControllerTests
    {
        [Fact]
        public async Task RecognizeWithStreamAsync_WithCorrectInput_ReturnsOk()
        {
            var list = new List<RecognizedCandidate>
            {
                new RecognizedCandidate { Confidence = 0.54, PersonId = $"{Guid.NewGuid()}" },
                new RecognizedCandidate { Confidence = 0.99, PersonId = $"{Guid.NewGuid()}" },
                new RecognizedCandidate { Confidence = 0.89, PersonId = $"{Guid.NewGuid()}" }
            };

            var mockLogger = Mock.Of<ILogger<FaceRecognitionController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.RecognizeWithStreamAsync(It.IsNotNull<Stream>()))
                .Returns(() => Task.FromResult((IList<RecognizedCandidate>)list));

            using var stream = File.OpenRead("jrtest.jpg");
            var controller = new FaceRecognitionController(mockFaceClientService.Object, mockLogger);
            var actionResult = await controller.RecognizeWithStreamAsync(stream);

            Assert.NotNull(actionResult);
            var okObjectResult = Assert.IsAssignableFrom<OkObjectResult>(actionResult);

            var result = Assert.IsAssignableFrom<IList<RecognizedCandidate>>(okObjectResult.Value);
            list.Should().NotBeEmpty()
                .And.BeEquivalentTo(result);
        }

        [Fact]
        public async Task RecognizeWithStreamAsync_WithUnrelatedStream_ReturnsOk()
        {
            var mockLogger = Mock.Of<ILogger<FaceRecognitionController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.RecognizeWithStreamAsync(It.IsNotNull<Stream>()));

            using var stream = new MemoryStream();
            var controller = new FaceRecognitionController(mockFaceClientService.Object, mockLogger);
            var actionResult = await controller.RecognizeWithStreamAsync(stream);

            Assert.NotNull(actionResult);
            var okObjectResult = Assert.IsAssignableFrom<OkObjectResult>(actionResult);

            var result = Assert.IsAssignableFrom<IList<RecognizedCandidate>>(okObjectResult.Value);
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task RecognizeWithStreamAsync_WithoutStream_ReturnsBadRequest()
        {
            var mockLogger = Mock.Of<ILogger<FaceRecognitionController>>();
            var mockFaceClientService = Mock.Of<IFaceClientService>();

            var controller = new FaceRecognitionController(mockFaceClientService, mockLogger);
            var actionResult = await controller.RecognizeWithStreamAsync(default);

            Assert.NotNull(actionResult);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task RecognizeWithStreamAsync_WithServiceFailure_ThrowException()
        {
            var mockLogger = Mock.Of<ILogger<FaceRecognitionController>>();
            var mockFaceClientService = new Mock<IFaceClientService>();
            mockFaceClientService
                .Setup(service => service.RecognizeWithStreamAsync(It.IsNotNull<Stream>()))
                .Throws<NotImplementedException>();

            using var stream = new MemoryStream();
            var controller = new FaceRecognitionController(mockFaceClientService.Object, mockLogger);
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                await controller.RecognizeWithStreamAsync(stream);
            });
        }
    }
}
