using ContactlessEntry.Cloud.Models.DataTransfer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class MaintenanceTests : IntegrationTestBase
    {
        public MaintenanceTests(ITestOutputHelper outputHelper, WebApplicationFactory<Startup> factory) : base(outputHelper, factory)
        {
        }

        [Fact]
        public async Task AddPerson_WithHappyPath_ReturnsOk()
        {
            var dto = new NewPersonDto
            {
                Name = $"Test-{Guid.NewGuid()}",
                FaceUrl = "https://media-exp1.licdn.com/dms/image/C4E03AQFAcf_JQva22Q/profile-displayphoto-shrink_200_200/0?e=1596067200&v=beta&t=4Zzskyqqlbig56lDTXwv8WY2Iyhjm4WAiGl4-GLUxKQ"
            };

            var token = await GetJwtTokenAsync();
            var person = await RequestService.PostAsync<NewPersonDto, Models.Person>("/v1/maintenance/person", dto, token);
            Assert.NotNull(person);
            Assert.NotNull(person.PersonId);
        }

        [Fact]
        public async Task RemovePerson_WithHappyPath_ReturnsOk()
        {
            var dto = new NewPersonDto
            {
                Name = $"Test-{Guid.NewGuid()}",
                FaceUrl = "https://media-exp1.licdn.com/dms/image/C4E03AQFAcf_JQva22Q/profile-displayphoto-shrink_200_200/0?e=1596067200&v=beta&t=4Zzskyqqlbig56lDTXwv8WY2Iyhjm4WAiGl4-GLUxKQ"
            };

            var token = await GetJwtTokenAsync();
            var person = await RequestService.PostAsync<NewPersonDto, Models.Person>("/v1/maintenance/person", dto, token);
            Assert.NotNull(person);
            Assert.NotNull(person.PersonId);

            await RequestService.DeleteAsync($"/v1/maintenance/person/{person.PersonId}");
        }

        [Fact]
        public async Task AddFace_WithHappyPath_ReturnsOk()
        {
            var dto = new NewPersonDto
            {
                Name = $"Test-{Guid.NewGuid()}",
                FaceUrl = "https://media-exp1.licdn.com/dms/image/C4E03AQFAcf_JQva22Q/profile-displayphoto-shrink_200_200/0?e=1596067200&v=beta&t=4Zzskyqqlbig56lDTXwv8WY2Iyhjm4WAiGl4-GLUxKQ"
            };

            var token = await GetJwtTokenAsync();
            var person = await RequestService.PostAsync<NewPersonDto, Models.Person>("/v1/maintenance/person", dto, token);
            Assert.NotNull(person);
            Assert.NotNull(person.PersonId);

            var faceDto = new FaceDto
            {
                FaceUrl = "https://media-exp1.licdn.com/dms/image/C4E03AQGKWV3Sg9INkg/profile-displayphoto-shrink_200_200/0?e=1598486400&v=beta&t=hcatALldZOD4LvVh16g_O1vNJcqd4JQNn0luyQUUkZc"
            };

            var face = await RequestService.PostAsync<FaceDto, Models.Face>($"/v1/maintenance/person/{person.PersonId}/faces", faceDto, token);
            Assert.NotNull(face);
            Assert.NotNull(face.FaceId);
        }

        [Fact]
        public async Task RemoveFace_WithHappyPath_ReturnsOk()
        {
            var dto = new NewPersonDto
            {
                Name = $"Test-{Guid.NewGuid()}",
                FaceUrl = "https://media-exp1.licdn.com/dms/image/C4E03AQFAcf_JQva22Q/profile-displayphoto-shrink_200_200/0?e=1596067200&v=beta&t=4Zzskyqqlbig56lDTXwv8WY2Iyhjm4WAiGl4-GLUxKQ"
            };

            var token = await GetJwtTokenAsync();

            var person = await RequestService.PostAsync<NewPersonDto, Models.Person>("/v1/maintenance/person", dto, token);
            Assert.NotNull(person);
            Assert.NotNull(person.PersonId);

            var faceDto = new FaceDto
            {
                FaceUrl = "https://media-exp1.licdn.com/dms/image/C4E03AQGKWV3Sg9INkg/profile-displayphoto-shrink_200_200/0?e=1598486400&v=beta&t=hcatALldZOD4LvVh16g_O1vNJcqd4JQNn0luyQUUkZc"
            };

            var face = await RequestService.PostAsync<FaceDto, Models.Face>($"/v1/maintenance/person/{person.PersonId}/faces", faceDto, token);
            Assert.NotNull(face);
            Assert.NotNull(face.FaceId);

            await RequestService.DeleteAsync($"/v1/maintenance/person/{person.PersonId}/faces/{face.FaceId}");
        }

        [Fact]
        public async Task GetTrainingStatus_WithHappyPath_ReturnsOk()
        {
            var token = await GetJwtTokenAsync();
            var trainingStatus = await RequestService.GetAsync<TrainingStatus>("/v1/maintenance/training", token);
            Assert.NotNull(trainingStatus);
        }
    }
}
