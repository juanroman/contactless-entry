using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.UnitTests.Utilities;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class AuthenticationTests : IntegrationTestBase
    {
        public AuthenticationTests(ITestOutputHelper outputHelper, TestWebApplicationFactory<Startup> factory) : base(outputHelper, factory)
        {
        }

        [Fact]
        public async Task Authenticate_WithInvalidApiCredentials_ReturnsBadRequest()
        {
            await RequestService.PostAsync<ApiCredentialsDto, AuthenticateResponseDto>("/v1/authenticate", default, handleResponse: (response) =>
            {
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                return true;
            });
        }

        [Fact]
        public async Task Authenticate_WithMissingApiKey_ReturnsBadRequest()
        {
            var dto = new ApiCredentialsDto
            {
                ApiKey = null,
                ApiSecret = $"{Guid.NewGuid()}"
            };

            await RequestService.PostAsync<ApiCredentialsDto, AuthenticateResponseDto>("/v1/authenticate", dto, handleResponse: (response) =>
            {
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                return true;
            });
        }

        [Fact]
        public async Task Authenticate_WithMissingApiSecret_ReturnsBadRequest()
        {
            var dto = new ApiCredentialsDto
            {
                ApiKey = $"{Guid.NewGuid()}",
                ApiSecret = null
            };

            await RequestService.PostAsync<ApiCredentialsDto, AuthenticateResponseDto>("/v1/authenticate", dto, handleResponse: (response) =>
            {
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                return true;
            });
        }

        [Fact]
        public async Task Authenticate_WithHappyPath_ReturnsOk()
        {
            var credentials = await RequestService.PostAsync<ApiCredentialsDto, AuthenticateResponseDto>("/v1/authenticate", new ApiCredentialsDto
            {
                ApiKey = $"{Guid.NewGuid()}",
                ApiSecret = $"{Guid.NewGuid()}"
            });

            Assert.NotNull(credentials);
            Assert.NotNull(credentials.Token);
        }
    }
}
