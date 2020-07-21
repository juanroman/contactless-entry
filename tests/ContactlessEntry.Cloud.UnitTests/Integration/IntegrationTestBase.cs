using ContactlessEntry.Cloud.Configuration;
using ContactlessEntry.Cloud.Models;
using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.UnitTests.Utilities;
using Divergic.Logging.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class IntegrationTestBase : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        public IntegrationTestBase(
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

        protected async Task BeginGroupTraining()
        {
            var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(MicroserviceSettings.FaceSubscriptionKey), new DelegatingHandler[] { })
            {
                Endpoint = MicroserviceSettings.FaceApiUrl,
            };

            var groups = await faceClient.PersonGroup.ListAsync();
            if (null != groups)
            {
                foreach (var item in groups)
                {
                    try
                    {
                        await faceClient.PersonGroup.TrainAsync(item.PersonGroupId);
                    }
                    catch (Exception)
                    {
                        // Safe to ignore
                    }
                }
            }
        }

        protected async Task<TrainingStatusType> GetTrainingStatus()
        {
            var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(MicroserviceSettings.FaceSubscriptionKey), new DelegatingHandler[] { })
            {
                Endpoint = MicroserviceSettings.FaceApiUrl,
            };

            var groups = await faceClient.PersonGroup.ListAsync();
            if (null != groups)
            {
                foreach (var item in groups)
                {
                    try
                    {
                        var trainingStatus = await faceClient.PersonGroup.GetTrainingStatusAsync(item.PersonGroupId);
                        return trainingStatus.Status;
                    }
                    catch (Exception)
                    {
                        return TrainingStatusType.Nonstarted;
                    }
                }
            }

            return TrainingStatusType.Nonstarted;
        }

        protected Task DeleteCognitiveServicesGroup()
        {
            //var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(MicroserviceSettings.FaceSubscriptionKey), new DelegatingHandler[] { })
            //{
            //    Endpoint = MicroserviceSettings.FaceApiUrl,
            //};

            //var groups = await faceClient.PersonGroup.ListAsync();
            //if (null != groups)
            //{
            //    foreach (var item in groups)
            //    {
            //        try
            //        {
            //            await faceClient.PersonGroup.DeleteAsync(item.PersonGroupId);
            //        }
            //        catch (Exception)
            //        {
            //            // Safe to ignore
            //        }
            //    }
            //}
            return Task.CompletedTask;
        }
    }
}
