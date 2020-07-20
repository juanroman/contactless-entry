using ContactlessEntry.Cloud.Models.DataTransfer;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class AuthenticationTests : IntegrationTestBase
    {
        public AuthenticationTests(ITestOutputHelper outputHelper, WebApplicationFactory<Startup> factory) : base(outputHelper, factory)
        {
        }

        [Fact]
        public async Task Authenticate_WithInvalidApiCredentials_ReturnsBadRequest()
        {

        }

        [Fact]
        public async Task Authenticate_WithMissingApiKey_ReturnsBadRequest()
        {

        }

        [Fact]
        public async Task Authenticate_WithMissingApiSecret_ReturnsBadRequest()
        {

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
