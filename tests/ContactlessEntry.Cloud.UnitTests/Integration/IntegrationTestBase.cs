using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.UnitTests.Utilities;
using Divergic.Logging.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Startup>>
    {
        public IntegrationTestBase(
            ITestOutputHelper outputHelper,
            WebApplicationFactory<Startup> factory)
        {
            var loggerFactory = LogFactory.Create(outputHelper);
            loggerFactory.AddProvider(new TestOutputLoggerProvider(outputHelper));

            var httpClient = factory.CreateClient();
            RequestService = new TestRequestService(httpClient, loggerFactory);
        }

        protected TestRequestService RequestService { get; private set; }

        protected async Task<string> GetJwtTokenAsync()
        {
            var credentials = await RequestService.PostAsync<ApiCredentialsDto, AuthenticateResponseDto>("/v1/authenticate", new ApiCredentialsDto
            {
                ApiKey = $"{Guid.NewGuid()}",
                ApiSecret = $"{Guid.NewGuid()}"
            });

            return credentials?.Token;
        }
    }
}
