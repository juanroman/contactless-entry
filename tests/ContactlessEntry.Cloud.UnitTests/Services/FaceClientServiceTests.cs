using ContactlessEntry.Cloud.Configuration;
using ContactlessEntry.Cloud.Services;
using FluentAssertions;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ContactlessEntry.Cloud.UnitTests.Services
{
    public class FaceClientServiceTests
    {
        private static readonly DelegatingHandler[] _handlers = new DelegatingHandler[] { };

        private readonly IMicroserviceSettings _microserviceSettings;
        private readonly ApiKeyServiceClientCredentials _apiKeyServiceClientCredentials;

        public FaceClientServiceTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            _microserviceSettings = configuration.GetSection(nameof(MicroserviceSettings)).Get<MicroserviceSettings>();
            _apiKeyServiceClientCredentials = new ApiKeyServiceClientCredentials(_microserviceSettings.FaceSubscriptionKey);
        }

        [Fact]
        public async Task BeginTrainingAsync_CompletesSuccessfully()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();

            var mockHttp = new MockHttpMessageHandler();
            MockGetGroup(mockHttp);
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*/train")
                    .Respond(HttpStatusCode.Accepted);

            var httpClient = mockHttp.ToHttpClient();

            var faceClient = new FaceClient(_apiKeyServiceClientCredentials, httpClient, true)
            {
                Endpoint = _microserviceSettings.FaceApiUrl
            };

            IFaceClientService service = new FaceClientService(faceClient, mockLogger, _microserviceSettings);
            await service.BeginTrainingAsync();
        }

        [Fact]
        public async Task GetTrainingStatusAsync_CompletesSuccessfully()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();

            var mockHttp = new MockHttpMessageHandler();
            MockGetGroup(mockHttp);
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*/training")
                    .Respond("application/json", "{" +
                        "'status': 'succeeded'," +
                        "'createdDateTime': '2018-10-15T11:51:27.6872495Z'," +
                        "'lastActionDateTime': '2018-10-15T11:51:27.8705696Z'," +
                        "'message': null}");

            var httpClient = mockHttp.ToHttpClient();

            var faceClient = new FaceClient(_apiKeyServiceClientCredentials, httpClient, true)
            {
                Endpoint = _microserviceSettings.FaceApiUrl
            };

            IFaceClientService service = new FaceClientService(faceClient, mockLogger, _microserviceSettings);
            var trainingStatus = await service.GetTrainingStatusAsync();
            Assert.NotNull(trainingStatus);
        }

        [Fact]
        public async Task GetTrainingStatusAsync_WithoutStatus_ReturnsNotFound()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();

            var mockHttp = new MockHttpMessageHandler();
            MockGetGroup(mockHttp);
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*/training")
                    .Respond(HttpStatusCode.NotFound, "application/json", "{" +
                        "'status': 'succeeded'," +
                        "'createdDateTime': '2018-10-15T11:51:27.6872495Z'," +
                        "'lastActionDateTime': '2018-10-15T11:51:27.8705696Z'," +
                        "'message': null}");

            var httpClient = mockHttp.ToHttpClient();

            var faceClient = new FaceClient(_apiKeyServiceClientCredentials, httpClient, true)
            {
                Endpoint = _microserviceSettings.FaceApiUrl
            };

            IFaceClientService service = new FaceClientService(faceClient, mockLogger, _microserviceSettings);
            var trainingStatus = await service.GetTrainingStatusAsync();
            Assert.NotNull(trainingStatus);
        }

        [Fact]
        public async Task GetTrainingStatusAsync_WithoutStatus_ThrowsServerError()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();

            var mockHttp = new MockHttpMessageHandler();
            MockGetGroup(mockHttp);
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*/training")
                    .Respond(HttpStatusCode.BadRequest, "application/json", "{" +
                        "'status': 'succeeded'," +
                        "'createdDateTime': '2018-10-15T11:51:27.6872495Z'," +
                        "'lastActionDateTime': '2018-10-15T11:51:27.8705696Z'," +
                        "'message': null}");

            var httpClient = mockHttp.ToHttpClient();

            var faceClient = new FaceClient(_apiKeyServiceClientCredentials, httpClient, true)
            {
                Endpoint = _microserviceSettings.FaceApiUrl
            };

            IFaceClientService service = new FaceClientService(faceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<APIErrorException>(async () =>
            {
                await service.GetTrainingStatusAsync();
            });
        }

        [Fact]
        public async Task CreatePersonAsync_WithCorrectInput_ReturnsPerson()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();

            var mockHttp = new MockHttpMessageHandler();
            MockGetGroup(mockHttp);
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*/persons")
                    .Respond("application/json", "{" +
                        "'personId': '25985303-c537-4467-b41d-bdb45cd95ca1'" +
                        "}");
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/compas/persons/*/persistedfaces")
                    .Respond("application/json", "{" +
                        "'persistedFaceId': 'B8D802CF-DD8F-4E61-B15C-9E6C5844CCBA'" +
                        "}");

            var httpClient = mockHttp.ToHttpClient();

            var faceClient = new FaceClient(_apiKeyServiceClientCredentials, httpClient, true)
            {
                Endpoint = _microserviceSettings.FaceApiUrl
            };

            IFaceClientService service = new FaceClientService(faceClient, mockLogger, _microserviceSettings);
            var person = await service.CreatePersonAsync($"{Guid.NewGuid()}", "https://media-exp1.licdn.com/dms/image/C4E03AQFAcf_JQva22Q/profile-displayphoto-shrink_200_200/0?e=1596067200&v=beta&t=4Zzskyqqlbig56lDTXwv8WY2Iyhjm4WAiGl4-GLUxKQ");
            person.Should().NotBeNull();
        }

        [Fact]
        public async Task CreatePersonAsync_WithMissingName_ThrowsArgumentNullException()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();
            var mockFaceClient = Mock.Of<IFaceClient>();

            IFaceClientService service = new FaceClientService(mockFaceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.CreatePersonAsync(null, $"{Guid.NewGuid()}");
            });
        }

        [Fact]
        public async Task CreatePersonAsync_WithMissingFaceUrl()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();
            var mockFaceClient = Mock.Of<IFaceClient>();

            IFaceClientService service = new FaceClientService(mockFaceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.CreatePersonAsync($"{Guid.NewGuid()}", null);
            });
        }

        [Fact]
        public async Task DeletePersonAsync_WithCorrectInput_ReturnsPerson()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();

            var mockHttp = new MockHttpMessageHandler();
            MockGetGroup(mockHttp);
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*/persons/*")
                    .Respond(HttpStatusCode.OK);

            var httpClient = mockHttp.ToHttpClient();

            var faceClient = new FaceClient(_apiKeyServiceClientCredentials, httpClient, true)
            {
                Endpoint = _microserviceSettings.FaceApiUrl
            };

            IFaceClientService service = new FaceClientService(faceClient, mockLogger, _microserviceSettings);
            await service.DeletePersonAsync($"{Guid.NewGuid()}");
        }

        [Fact]
        public async Task DeletePersonAsync_WithMissingPersonId_ThrowsArgumentNullException()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();
            var mockFaceClient = Mock.Of<IFaceClient>();

            IFaceClientService service = new FaceClientService(mockFaceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.DeletePersonAsync(null);
            });
        }

        [Fact]
        public async Task DeletePersonAsync_WithInvalidPersonId_ThrowsArgumentException()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();
            var mockFaceClient = Mock.Of<IFaceClient>();

            IFaceClientService service = new FaceClientService(mockFaceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await service.DeletePersonAsync("12345");
            });
        }

        [Fact]
        public async Task AddFaceAsync_WithCorrectInput_ReturnsPerson()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();

            var mockHttp = new MockHttpMessageHandler();
            MockGetGroup(mockHttp);
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/compas/persons/*/persistedfaces")
                    .Respond("application/json", "{" +
                        "'persistedFaceId': 'B8D802CF-DD8F-4E61-B15C-9E6C5844CCBA'" +
                        "}");

            var httpClient = mockHttp.ToHttpClient();

            var faceClient = new FaceClient(_apiKeyServiceClientCredentials, httpClient, true)
            {
                Endpoint = _microserviceSettings.FaceApiUrl
            };

            IFaceClientService service = new FaceClientService(faceClient, mockLogger, _microserviceSettings);
            var face = await service.AddFaceAsync($"{Guid.NewGuid()}", "https://media-exp1.licdn.com/dms/image/C4E03AQFAcf_JQva22Q/profile-displayphoto-shrink_200_200/0?e=1596067200&v=beta&t=4Zzskyqqlbig56lDTXwv8WY2Iyhjm4WAiGl4-GLUxKQ");
            face.Should().NotBeNull();
        }

        [Fact]
        public async Task AddFaceAsync_WithMissingPersonId_ThrowsArgumentNullException()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();
            var mockFaceClient = Mock.Of<IFaceClient>();

            IFaceClientService service = new FaceClientService(mockFaceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.AddFaceAsync(null, "https://media-exp1.licdn.com/dms/image/C4E03AQFAcf_JQva22Q/profile-displayphoto-shrink_200_200/0?e=1596067200&v=beta&t=4Zzskyqqlbig56lDTXwv8WY2Iyhjm4WAiGl4-GLUxKQ");
            });
        }

        [Fact]
        public async Task AddFaceAsync_WithInvalidPersonId_ThrowsArgumentException()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();
            var mockFaceClient = Mock.Of<IFaceClient>();

            IFaceClientService service = new FaceClientService(mockFaceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await service.AddFaceAsync("12345", "https://media-exp1.licdn.com/dms/image/C4E03AQFAcf_JQva22Q/profile-displayphoto-shrink_200_200/0?e=1596067200&v=beta&t=4Zzskyqqlbig56lDTXwv8WY2Iyhjm4WAiGl4-GLUxKQ");
            });
        }

        [Fact]
        public async Task AddFaceAsync_WithMissingFaceUrl_ThrowsArgumentNullException()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();
            var mockFaceClient = Mock.Of<IFaceClient>();

            IFaceClientService service = new FaceClientService(mockFaceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.AddFaceAsync($"{Guid.NewGuid()}", null);
            });
        }

        [Fact]
        public async Task RemoveFaceAsync_WithCorrectInput_ReturnsPerson()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();

            var mockHttp = new MockHttpMessageHandler();
            MockGetGroup(mockHttp);
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*/persons/*/persistedfaces/*")
                    .Respond(HttpStatusCode.OK);

            var httpClient = mockHttp.ToHttpClient();

            var faceClient = new FaceClient(_apiKeyServiceClientCredentials, httpClient, true)
            {
                Endpoint = _microserviceSettings.FaceApiUrl
            };

            IFaceClientService service = new FaceClientService(faceClient, mockLogger, _microserviceSettings);
            await service.RemoveFaceAsync($"{Guid.NewGuid()}", $"{Guid.NewGuid()}");
        }

        [Fact]
        public async Task RemoveFaceAsync_WithMissingPersonId_ThrowsArgumentNullException()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();
            var mockFaceClient = Mock.Of<IFaceClient>();

            IFaceClientService service = new FaceClientService(mockFaceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.RemoveFaceAsync(null, $"{Guid.NewGuid()}");
            });
        }

        [Fact]
        public async Task RemoveFaceAsync_WithMissingFaceId_ThrowsArgumentNullException()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();
            var mockFaceClient = Mock.Of<IFaceClient>();

            IFaceClientService service = new FaceClientService(mockFaceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.RemoveFaceAsync($"{Guid.NewGuid()}", null);
            });
        }

        [Fact]
        public async Task RemoveFaceAsync_WithInvalidPersonId_ThrowsArgumentException()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();
            var mockFaceClient = Mock.Of<IFaceClient>();

            IFaceClientService service = new FaceClientService(mockFaceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await service.RemoveFaceAsync("1234", $"{Guid.NewGuid()}");
            });
        }

        [Fact]
        public async Task RemoveFaceAsync_WithInvalidFaceId_ThrowsArgumentException()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();
            var mockFaceClient = Mock.Of<IFaceClient>();

            IFaceClientService service = new FaceClientService(mockFaceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await service.RemoveFaceAsync($"{Guid.NewGuid()}", "1234");
            });
        }

        [Fact]
        public async Task BeginTrainingAsync_WithoutGroup_ForcesGroupCreation()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*?returnRecognitionModel=false")
                    .Respond(HttpStatusCode.NotFound, "application/json", "{" +
                        "'personGroupId': 'sample_group'," +
                        "'name': 'group1'," +
                        "'userData': 'User-provided data attached to the person group.'," +
                        "'recognitionModel': 'recognition_02'}");
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*")
                    .Respond(HttpStatusCode.OK);
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/compas/training")
                    .Respond("application/json", "{" +
                        "'status': 'succeeded'," +
                        "'createdDateTime': '2018-10-15T11:51:27.6872495Z'," +
                        "'lastActionDateTime': '2018-10-15T11:51:27.8705696Z'," +
                        "'message': null}");

            var httpClient = mockHttp.ToHttpClient();

            var faceClient = new FaceClient(_apiKeyServiceClientCredentials, httpClient, true)
            {
                Endpoint = _microserviceSettings.FaceApiUrl
            };

            IFaceClientService service = new FaceClientService(faceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await service.GetTrainingStatusAsync();
            });
        }

        [Fact]
        public async Task BeginTrainingAsync_WithServerError_ThrowsException()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*?returnRecognitionModel=false")
                    .Respond(HttpStatusCode.BadRequest, "application/json", "{" +
                        "'personGroupId': 'sample_group'," +
                        "'name': 'group1'," +
                        "'userData': 'User-provided data attached to the person group.'," +
                        "'recognitionModel': 'recognition_02'}");
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*")
                    .Respond(HttpStatusCode.OK);
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/compas/training")
                    .Respond("application/json", "{" +
                        "'status': 'succeeded'," +
                        "'createdDateTime': '2018-10-15T11:51:27.6872495Z'," +
                        "'lastActionDateTime': '2018-10-15T11:51:27.8705696Z'," +
                        "'message': null}");

            var httpClient = mockHttp.ToHttpClient();

            var faceClient = new FaceClient(_apiKeyServiceClientCredentials, httpClient, true)
            {
                Endpoint = _microserviceSettings.FaceApiUrl
            };

            IFaceClientService service = new FaceClientService(faceClient, mockLogger, _microserviceSettings);
            await Assert.ThrowsAsync<APIErrorException>(async () =>
            {
                await service.GetTrainingStatusAsync();
            });
        }

        [Fact]
        public async Task RecognizeWithStreamAsync_WithCorrectInput_ReturnsCandidateList()
        {
            var mockLogger = Mock.Of<ILogger<FaceClientService>>();

            var mockHttp = new MockHttpMessageHandler();
            MockGetGroup(mockHttp);
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=true&recognitionModel=recognition_02&returnRecognitionModel=false")
                    .Respond("application/json",
                        "[" +
                            "{" +
                                "'faceId': 'c5c24a82-6845-4031-9d5d-978df9175426'," +
                                "'recognitionModel': 'recognition_02'," +
                                "'faceRectangle': { 'width': 78, 'height': 78, 'left': 394, 'top': 54" +
                                "}," +
                                "'faceLandmarks': {" +
                                    "'pupilLeft': { 'x': 412.7, 'y': 78.4 }," +
                                    "'pupilRight': { 'x': 446.8, 'y': 74.2 }," +
                                    "'noseTip': { 'x': 437.7, 'y': 92.4 }," +
                                    "'mouthLeft': { 'x': 417.8, 'y': 114.4 }," +
                                    "'mouthRight': { 'x': 451.3, 'y': 109.3 }," +
                                    "'eyebrowLeftOuter': { 'x': 397.9, 'y': 78.5 }," +
                                    "'eyebrowLeftInner': { 'x': 425.4, 'y': 70.5 }," +
                                    "'eyeLeftOuter': { 'x': 406.7, 'y': 80.6 }," +
                                    "'eyeLeftTop': { 'x': 412.2, 'y': 76.2 }," +
                                    "'eyeLeftBottom': { 'x': 413.0, 'y': 80.1 }," +
                                    "'eyeLeftInner': { 'x': 418.9, 'y': 78.0 }," +
                                    "'eyebrowRightInner': { 'x': 4.8, 'y': 69.7 }," +
                                    "'eyebrowRightOuter': { 'x': 5.5, 'y': 68.5 }," +
                                    "'eyeRightInner': { 'x': 441.5, 'y': 75.0 }," +
                                    "'eyeRightTop': { 'x': 446.4, 'y': 71.7 }," +
                                    "'eyeRightBottom': { 'x': 447.0, 'y': 75.3 }," +
                                    "'eyeRightOuter': { 'x': 451.7, 'y': 73.4 }," +
                                    "'noseRootLeft': {'x': 428.0, 'y': 77.1 }," +
                                    "'noseRootRight': { 'x': 435.8, 'y': 75.6 }," +
                                    "'noseLeftAlarTop': { 'x': 428.3, 'y': 89.7 }," +
                                    "'noseRightAlarTop': { 'x': 442.2, 'y': 87.0 }," +
                                    "'noseLeftAlarOutTip': { 'x': 424.3, 'y': 96.4 }," +
                                    "'noseRightAlarOutTip': { 'x': 446.6, 'y': 92.5 }," +
                                    "'upperLipTop': { 'x': 437.6, 'y': 105.9 }," +
                                    "'upperLipBottom': { 'x': 437.6, 'y': 108.2 }," +
                                    "'underLipTop': { 'x': 436.8, 'y': 111.4 }," +
                                    "'underLipBottom': { 'x': 437.3, 'y': 114.5 }" +
                                "}," +
                                "'faceAttributes': {" +
                                    "'age': 71.0," +
                                    "'gender': 'male'," +
                                    "'smile': 0.88," +
                                    "'facialHair': { 'moustache': 0.8, 'beard': 0.1, 'sideburns': 0.02 }," +
                                    "'glasses': 'sunglasses'," +
                                    "'headPose': { 'roll': 2.1, 'yaw': 3, 'pitch': 1.6 }," +
                                    "'emotion': { 'anger': 0.575, 'contempt': 0, 'disgust': 0.006, 'fear': 0.008, 'happiness': 0.394, 'neutral': 0.013, 'sadness': 0, 'surprise': 0.004 }," +
                                    "'hair': {" +
                                        "'bald': 0.0," +
                                        "'invisible': false," +
                                        "'hairColor': [" +
                                            "{ 'color': 'brown', 'confidence': 1.0}," +
                                            "{ 'color': 'blond', 'confidence': 0.88}," +
                                            "{ 'color': 'black', 'confidence': 0.48}," +
                                            "{ 'color': 'other', 'confidence': 0.11}," +
                                            "{ 'color': 'gray', 'confidence': 0.07}," +
                                            "{ 'color': 'red', 'confidence': 0.03}" +
                                        "]" +
                                    "}," +
                                    "'makeup': {" +
                                        "'eyeMakeup': true," +
                                        "'lipMakeup': false" +
                                    "}," +
                                    "'occlusion': {" +
                                        "'foreheadOccluded': false," +
                                        "'eyeOccluded': false," +
                                        "'mouthOccluded': false" +
                                    "}," +
                                    "'accessories': [" +
                                        "{'type': 'headWear', 'confidence': 0.99}," +
                                        "{ 'type': 'glasses', 'confidence': 1.0}," +
                                        "{ 'type': 'mask',' confidence': 0.87}" +
                                    "]," +
                                    "'blur': {" +
                                        "'blurLevel': 'Medium'," +
                                        "'value': 0.51" +
                                    "}," +
                                    "'exposure': {" +
                                        "'exposureLevel': 'GoodExposure'," +
                                        "'value': 0.55" +
                                    "}," +
                                    "'noise': {" +
                                        "'noiseLevel': 'Low'," +
                                        "'value': 0.12" +
                                    "}" +
                                "}" +
                            "}" +
                        "]");
            mockHttp.When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/identify")
                    .Respond("application/json",
                        "[" +
                            "{" +
                                "'faceId': 'c5c24a82-6845-4031-9d5d-978df9175426'," +
                                "'candidates': [ { 'personId': '25985303-c537-4467-b41d-bdb45cd95ca1', 'confidence': 0.92 } ]" +
                            "}," +
                            "{" +
                                "'faceId': '65d083d4-9447-47d1-af30-b626144bf0fb'," +
                                "'candidates': [ { 'personId': '2ae4935b-9659-44c3-977f-61fac20d0538', 'confidence': 0.89 } ]" +
                            "}" +
                        "]");

            var httpClient = mockHttp.ToHttpClient();

            var faceClient = new FaceClient(_apiKeyServiceClientCredentials, httpClient, true)
            {
                Endpoint = _microserviceSettings.FaceApiUrl
            };

            using var stream = File.OpenRead("jrtest.jpg");

            IFaceClientService service = new FaceClientService(faceClient, mockLogger, _microserviceSettings);
            var candidates = await service.RecognizeWithStreamAsync(stream);
        }

        private void MockGetGroup(MockHttpMessageHandler mockHttp) => mockHttp
            .When($"{_microserviceSettings.FaceApiUrl}/face/v1.0/persongroups/*?returnRecognitionModel=false")
            .Respond("application/json", "{" +
                "'personGroupId': 'sample_group'," +
                "'name': 'group1'," +
                "'userData': 'User-provided data attached to the person group.'," +
                "'recognitionModel': 'recognition_02'}");
    }
}
