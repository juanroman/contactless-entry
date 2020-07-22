using ContactlessEntry.Cloud.Configuration;
using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.UnitTests.Utilities;
using Divergic.Logging.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
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

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            MicroserviceSettings = configuration.GetSection(nameof(MicroserviceSettings)).Get<MicroserviceSettings>();
        }

        protected MicroserviceSettings MicroserviceSettings { get; private set; }

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
