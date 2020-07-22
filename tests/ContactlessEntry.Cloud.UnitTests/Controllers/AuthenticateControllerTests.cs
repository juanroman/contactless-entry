using ContactlessEntry.Cloud.Configuration;
using ContactlessEntry.Cloud.Controllers;
using ContactlessEntry.Cloud.Models.DataTransfer;
using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;

namespace ContactlessEntry.Cloud.UnitTests.Controllers
{
    public class AuthenticateControllerTests
    {
        [Fact]
        public void Authenticate_WithCorrectInput_ReturnsOk()
        {
            IMicroserviceSettings microserviceSettings = new MicroserviceSettings
            {
                JwtIssuer = $"{Guid.NewGuid()}",
                JwtKey = $"{Guid.NewGuid()}"
            };

            var dto = new ApiCredentialsDto
            {
                ApiKey = $"{Guid.NewGuid()}",
                ApiSecret = $"{Guid.NewGuid()}"
            };

            var controller = new AuthenticateController(microserviceSettings);
            var actionResult = controller.Authenticate(dto);
            Assert.NotNull(actionResult);

            var okObjectResult = Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            var response = Assert.IsAssignableFrom<AuthenticateResponseDto>(okObjectResult?.Value);
            Assert.NotNull(response);
            Assert.NotNull(response.Token);
        }

        [Fact]
        public void Authenticate_WithIncorrectInput_ReturnsBadRequest()
        {
            IMicroserviceSettings microserviceSettings = new MicroserviceSettings
            {
                JwtIssuer = $"{Guid.NewGuid()}",
                JwtKey = $"{Guid.NewGuid()}"
            };

            var controller = new AuthenticateController(microserviceSettings);
            var actionResult = controller.Authenticate(null);
            Assert.NotNull(actionResult);

            Assert.IsAssignableFrom<BadRequestResult>(actionResult);
        }

        [Fact]
        public void Authenticate_WithMissingApiSecret_ReturnsUnauthorized()
        {
            IMicroserviceSettings microserviceSettings = new MicroserviceSettings
            {
                JwtIssuer = $"{Guid.NewGuid()}",
                JwtKey = $"{Guid.NewGuid()}"
            };

            var dto = new ApiCredentialsDto
            {
                ApiKey = $"{Guid.NewGuid()}",
                ApiSecret = string.Empty
            };

            var controller = new AuthenticateController(microserviceSettings);
            var actionResult = controller.Authenticate(dto);
            Assert.NotNull(actionResult);

            Assert.IsAssignableFrom<UnauthorizedResult>(actionResult);
        }

        [Fact]
        public void Authenticate_WithMissingApiKey_ReturnsUnauthorized()
        {
            IMicroserviceSettings microserviceSettings = new MicroserviceSettings
            {
                JwtIssuer = $"{Guid.NewGuid()}",
                JwtKey = $"{Guid.NewGuid()}"
            };

            var dto = new ApiCredentialsDto
            {
                ApiKey = string.Empty,
                ApiSecret = $"{Guid.NewGuid()}"
            };

            var controller = new AuthenticateController(microserviceSettings);
            var actionResult = controller.Authenticate(dto);
            Assert.NotNull(actionResult);

            Assert.IsAssignableFrom<UnauthorizedResult>(actionResult);
        }
    }
}
