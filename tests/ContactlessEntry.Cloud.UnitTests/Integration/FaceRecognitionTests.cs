using ContactlessEntry.Cloud.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class FaceRecognitionTests : IntegrationTestBase
    {
        public FaceRecognitionTests(ITestOutputHelper outputHelper, WebApplicationFactory<Startup> factory) : base(outputHelper, factory)
        {
        }

        [Fact]
        public async Task FaceRecognition_WithHappyPath_ReturnsOk()
        {
            using var stream = File.OpenRead("jrtest.jpg");

            var token = await GetJwtTokenAsync();

            var candidates = await RequestService.PostStreamAsync<List<RecognizedCandidate>>("/v1/facerecognition", stream, token);
            Assert.NotNull(candidates);
        }

        [Fact]
        public async Task FaceRecognition_WithInvalidModel_ReturnsBadRequest()
        {

        }

        [Fact]
        public async Task FaceRecognition_WithUnknownFace_ReturnsNotFound()
        {

        }
    }
}
