using AutoMapper;
using ContactlessEntry.Cloud.Controllers;
using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.Services;
using ContactlessEntry.Cloud.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ContactlessEntry.Cloud.UnitTests.Controllers
{
    public class AccessControllerTests
    {
        [Fact]
        public async Task RequestAccessAsync_WithCorrectInput_ReturnsOk()
        {
            var mapper = new MapperConfiguration(mapperConfiguration => mapperConfiguration.AddProfile(new AutoMapperProfile())).CreateMapper();
            var accessManager = Mock.Of<IAccessManager>();
            var logger = Mock.Of<ILogger<AccessController>>();

            var dto = new RequestAccessDto
            {
                DoorId = $"{Guid.NewGuid()}",
                PersonId = $"{Guid.NewGuid()}",
                Temperature = 37
            };

            var controller = new AccessController(mapper, accessManager, logger);
            var actionResult = await controller.RequestAccessAsync(dto);
            Assert.NotNull(actionResult);
            Assert.IsAssignableFrom<OkObjectResult>(actionResult);
        }

        [Fact]
        public async Task RequestAccessAsync_WithBadInput_ReturnsBadRequest()
        {
            var mapper = new MapperConfiguration(mapperConfiguration => mapperConfiguration.AddProfile(new AutoMapperProfile())).CreateMapper();
            var accessManager = Mock.Of<IAccessManager>();
            var logger = Mock.Of<ILogger<AccessController>>();

            var controller = new AccessController(mapper, accessManager, logger);
            var actionResult = await controller.RequestAccessAsync(null);
            Assert.NotNull(actionResult);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actionResult);
        }
    }
}
