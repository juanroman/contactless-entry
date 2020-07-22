using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class SwaggerGenerationTests : IntegrationTestBase
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;

        public SwaggerGenerationTests(ITestOutputHelper outputHelper, WebApplicationFactory<Startup> factory) : base(outputHelper, factory)
        {
            _webApplicationFactory = factory;
        }

        [Fact]
        public async Task Swagger_GetUI()
        {
            var response = await _webApplicationFactory.CreateClient().GetAsync("/index.html");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Swagger_GetJson()
        {
            var response = await _webApplicationFactory.CreateClient().GetAsync("/swagger/v1/swagger.json");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
