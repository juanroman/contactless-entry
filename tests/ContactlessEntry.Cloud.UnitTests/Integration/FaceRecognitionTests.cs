using ContactlessEntry.Cloud.Configuration;
using ContactlessEntry.Cloud.Models;
using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.UnitTests.Utilities;
using Divergic.Logging.Xunit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class FaceRecognitionTests : IClassFixture<FaceRecognitionFixture>
    {
        private readonly FaceRecognitionFixture _fixture;

        public FaceRecognitionTests(FaceRecognitionFixture fixture)
        {
            _fixture = fixture;
        }

        //[Fact]
        //public async Task FaceRecognition_WithHappyPath_ReturnsOk()
        //{
        //    var token = await _fixture.GetJwtTokenAsync();
        //    using var stream = File.OpenRead("jrtest.jpg");

        //    var candidates = await _fixture.RequestService.PostStreamAsync<List<RecognizedCandidate>>("/v1/facerecognition", stream, token);
        //    Assert.NotNull(candidates);
        //}

        //[Fact]
        //public void FaceRecognition_WithInvalidModel_ReturnsBadRequest()
        //{

        //}

        //[Fact]
        //public void FaceRecognition_WithUnknownFace_ReturnsNotFound()
        //{

        //}
    }

    public class FaceRecognitionFixture : IAsyncLifetime
    {
        public FaceRecognitionFixture(
            ITestOutputHelper outputHelper,
            TestWebApplicationFactory<Startup> factory)
        {
            var loggerFactory = LogFactory.Create(outputHelper);
            loggerFactory.AddProvider(new TestOutputLoggerProvider(outputHelper));

            factory.Output = outputHelper;

            var httpClient = factory.CreateClient();
            RequestService = new TestRequestService(httpClient, loggerFactory);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            MicroserviceSettings = configuration
                .GetSection(nameof(MicroserviceSettings))
                .Get<MicroserviceSettings>();

            PersonId = $"{nameof(FaceRecognitionFixture)}-{Guid.NewGuid()}";
        }

        public MicroserviceSettings MicroserviceSettings { get; private set; }

        public TestRequestService RequestService { get; private set; }

        public string PersonId { get; set; }

        public async Task InitializeAsync()
        {
            // Create Recoginizable person
            var dto = new NewPersonDto
            {
                Name = PersonId,
                FaceUrl = "https://media-exp1.licdn.com/dms/image/C4E03AQFAcf_JQva22Q/profile-displayphoto-shrink_200_200/0?e=1596067200&v=beta&t=4Zzskyqqlbig56lDTXwv8WY2Iyhjm4WAiGl4-GLUxKQ"
            };

            var token = await GetJwtTokenAsync();
            await RequestService.PostAsync<NewPersonDto, Person>("/v1/maintenance/person", dto, token);

            // Ensure gropu is trained
            await RequestService.PostAsync("/v1/maintenance/training", default(object), token);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<string> GetJwtTokenAsync()
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
