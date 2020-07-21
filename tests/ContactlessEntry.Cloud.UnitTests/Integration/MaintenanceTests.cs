using ContactlessEntry.Cloud.UnitTests.Utilities;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class MaintenanceTests : IntegrationTestBase, IAsyncLifetime
    {
        public MaintenanceTests(ITestOutputHelper outputHelper, TestWebApplicationFactory<Startup> factory) : base(outputHelper, factory)
        {
        }

        //[Fact]
        //public async Task AddPerson_WithHappyPath_ReturnsOk()
        //{
        //    var dto = new NewPersonDto
        //    {
        //        Name = $"Test-{Guid.NewGuid()}",
        //        FaceUrl = "https://media-exp1.licdn.com/dms/image/C4E03AQFAcf_JQva22Q/profile-displayphoto-shrink_200_200/0?e=1596067200&v=beta&t=4Zzskyqqlbig56lDTXwv8WY2Iyhjm4WAiGl4-GLUxKQ"
        //    };

        //    var token = await GetJwtTokenAsync();
        //    var person = await RequestService.PostAsync<NewPersonDto, Models.Person>("/v1/maintenance/person", dto, token);
        //    Assert.NotNull(person);
        //    Assert.NotNull(person.PersonId);
        //}

        //[Fact]
        //public async Task RemovePerson_WithHappyPath_ReturnsOk()
        //{
        //    var dto = new NewPersonDto
        //    {
        //        Name = $"Test-{Guid.NewGuid()}",
        //        FaceUrl = "https://thumbor.forbes.com/thumbor/fit-in/416x416/filters%3Aformat%28jpg%29/https%3A%2F%2Fspecials-images.forbesimg.com%2Fimageserve%2F5d6ae14f673aa300083caf99%2F0x0.jpg%3Fbackground%3D000000%26cropX1%3D3051%26cropX2%3D5974%26cropY1%3D26%26cropY2%3D2952"
        //    };

        //    var token = await GetJwtTokenAsync();
        //    var person = await RequestService.PostAsync<NewPersonDto, Models.Person>("/v1/maintenance/person", dto, token);
        //    Assert.NotNull(person);
        //    Assert.NotNull(person.PersonId);

        //    await RequestService.DeleteAsync($"/v1/maintenance/person/{person.PersonId}");
        //}

        //[Fact]
        //public async Task AddFace_WithHappyPath_ReturnsOk()
        //{
        //    var dto = new NewPersonDto
        //    {
        //        Name = $"Test-{Guid.NewGuid()}",
        //        FaceUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/ed/Elon_Musk_Royal_Society.jpg/250px-Elon_Musk_Royal_Society.jpg"
        //    };

        //    var token = await GetJwtTokenAsync();
        //    var person = await RequestService.PostAsync<NewPersonDto, Models.Person>("/v1/maintenance/person", dto, token);
        //    Assert.NotNull(person);
        //    Assert.NotNull(person.PersonId);

        //    var faceDto = new FaceDto
        //    {
        //        FaceUrl = "https://media-exp1.licdn.com/dms/image/C4E03AQGKWV3Sg9INkg/profile-displayphoto-shrink_200_200/0?e=1598486400&v=beta&t=hcatALldZOD4LvVh16g_O1vNJcqd4JQNn0luyQUUkZc"
        //    };

        //    var face = await RequestService.PostAsync<FaceDto, Models.Face>($"/v1/maintenance/person/{person.PersonId}/faces", faceDto, token);
        //    Assert.NotNull(face);
        //    Assert.NotNull(face.FaceId);
        //}

        //[Fact]
        //public async Task RemoveFace_WithHappyPath_ReturnsOk()
        //{
        //    var dto = new NewPersonDto
        //    {
        //        Name = $"Test-{Guid.NewGuid()}",
        //        FaceUrl = "https://image.cnbcfm.com/api/v1/image/106569797-1591649109683gettyimages-1032942656.jpeg?v=1594382862&w=1400&h=950"
        //    };

        //    var token = await GetJwtTokenAsync();

        //    var person = await RequestService.PostAsync<NewPersonDto, Models.Person>("/v1/maintenance/person", dto, token);
        //    Assert.NotNull(person);
        //    Assert.NotNull(person.PersonId);

        //    var faceDto = new FaceDto
        //    {
        //        FaceUrl = "https://www.infotechnology.com/__export/1593784102938/sites/revistait/img/2020/07/03/jeff-bezos-kcbg--620x349xabc.jpg_147459497.jpg"
        //    };

        //    var face = await RequestService.PostAsync<FaceDto, Models.Face>($"/v1/maintenance/person/{person.PersonId}/faces", faceDto, token);
        //    Assert.NotNull(face);
        //    Assert.NotNull(face.FaceId);

        //    await RequestService.DeleteAsync($"/v1/maintenance/person/{person.PersonId}/faces/{face.FaceId}");
        //}

        //[Fact]
        //public async Task GetTrainingStatus_WithHappyPath_ReturnsOk()
        //{
        //    var token = await GetJwtTokenAsync();
        //    var trainingStatus = await RequestService.GetAsync<TrainingStatus>("/v1/maintenance/training", token);
        //    Assert.NotNull(trainingStatus);
        //}

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await DeleteCognitiveServicesGroup();
        }
    }
}
