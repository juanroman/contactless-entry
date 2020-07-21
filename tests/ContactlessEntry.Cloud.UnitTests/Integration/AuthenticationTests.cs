using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.UnitTests.Utilities;
using System;
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
        public void Authenticate_WithInvalidApiCredentials_ReturnsBadRequest()
        {

        }

        [Fact]
        public void Authenticate_WithMissingApiKey_ReturnsBadRequest()
        {

        }

        [Fact]
        public void Authenticate_WithMissingApiSecret_ReturnsBadRequest()
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
