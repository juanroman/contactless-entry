using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.UnitTests.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class RequestAccessTests : IntegrationTestBase
    {
        public RequestAccessTests(ITestOutputHelper outputHelper, TestWebApplicationFactory<Startup> factory) : base(outputHelper, factory)
        {
        }

        [Fact]
        public async Task RequestAccess_WithoutToken_ReturnsUnauthorized()
        {
            var model = new
            {
                DoorId = $"{Guid.NewGuid()}",
                PersonId = $"{Guid.NewGuid()}",
                Temperature = 0
            };

            await RequestService.PostAsync("/v1/access/request", model, handleResponse: (response) =>
            {
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
                return true;
            });
        }

        [Fact]
        public async Task RequestAccess_WithInvalidToken_ReturnsUnauthorized()
        {
            var model = new
            {
                DoorId = $"{Guid.NewGuid()}",
                PersonId = $"{Guid.NewGuid()}",
                Temperature = 0
            };

            await RequestService.PostAsync("/v1/access/request", model, $"{Guid.NewGuid()}", handleResponse: (response) =>
            {
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
                return true;
            });
        }

        [Fact]
        public async Task RequestAccess_WithInvalidDoorId_ReturnsBadRequest()
        {
            var model = new
            {
                DoorId = string.Empty,
                PersonId = $"{Guid.NewGuid()}",
                Temperature = 0
            };

            var token = await GetJwtTokenAsync();
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await RequestService.PostAsync("/v1/access/request", model, token, handleResponse: (response) =>
                {
                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                    return false;
                });
            });
        }

        [Fact]
        public async Task RequestAccess_WithInvalidPersonId_ReturnsBadRequest()
        {
            var model = new
            {
                DoorId = $"{Guid.NewGuid()}",
                PersonId = string.Empty,
                Temperature = 0
            };

            var token = await GetJwtTokenAsync();
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await RequestService.PostAsync("/v1/access/request", model, token, handleResponse: (response) =>
                {
                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                    return false;
                });
            });
        }

        [Fact]
        public async Task RequestAccess_WithInvalidTemperature_ReturnsBadRequest()
        {
            var model = new
            {
                DoorId = $"{Guid.NewGuid()}",
                PersonId = $"{Guid.NewGuid()}"
            };

            var token = await GetJwtTokenAsync();
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await RequestService.PostAsync("/v1/access/request", model, token, handleResponse: (response) =>
                {
                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                    return false;
                });
            });
        }

        [Fact]
        public async Task RequestAccess_WithHappyPath_ReturnsOk()
        {
            var model = new RequestAccessDto
            {
                DoorId = $"{Guid.NewGuid()}",
                PersonId = $"{Guid.NewGuid()}",
                Temperature = 37.53
            };

            var token = await GetJwtTokenAsync();
            var access = await RequestService.PostAsync<RequestAccessDto, AccessDto>("/v1/access/request", model, token, handleResponse: (response) =>
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                return false;
            });

            Assert.NotNull(access);

            await DeleteCognitiveServicesGroup();
        }
    }
}
