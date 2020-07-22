using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class AuthenticateTests : IntegrationTestBase
    {
        public AuthenticateTests(ITestOutputHelper outputHelper, WebApplicationFactory<Startup> factory) : base(outputHelper, factory)
        {
        }

        [Fact]
        public async Task AuthenticatePost_WithHappyPath_ReturnsOk()
        {
            var dto = new
            {
                apiKey = $"{Guid.NewGuid()}",
                apiSecret = $"{Guid.NewGuid()}"
            };

            var response = await RequestService.PostAsync<object, AuthenticateResponse>("/v1/authenticate", dto);
            response.Should().NotBeNull();
            response.Token.Should().NotBeNullOrWhiteSpace();
        }

        class AuthenticateResponse
        {
            public string Token { get; set; }
        }
    }
}
