using ContactlessEntry.Cloud.Configuration;
using ContactlessEntry.Cloud.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContactlessEntry.Cloud.Services
{
    public class FaceClientService : IFaceClientService
    {
        public const string PersonGroupId = "compas";

        private readonly IFaceClient _faceClient;
        private readonly ILogger<FaceClientService> _logger;
        private readonly IMicroserviceSettings _microserviceSettings;

        public FaceClientService(
            ILogger<FaceClientService> logger,
            IMicroserviceSettings microserviceSettings)
        {
            _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(microserviceSettings.FaceSubscriptionKey), new DelegatingHandler[] { })
            {
                Endpoint = microserviceSettings.FaceApiUrl,
            };

            _logger = logger;
            _microserviceSettings = microserviceSettings;
        }

        public async Task<IList<RecognizedCandidate>> RecognizeWithStreamAsync(Stream image)
        {
            if (null == image)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var recognizedCandidateList = new List<RecognizedCandidate>();

            var detectedFaces = await _faceClient.Face.DetectWithStreamAsync(image, true, true, null, _microserviceSettings.RecognitionModel);
            if (null != detectedFaces)
            {
                var faceGuids = detectedFaces
                    .Where(face => face.FaceId.HasValue)
                    .Select(face => face.FaceId.Value)
                    .ToList();
                if (null != faceGuids && faceGuids.Any())
                {
                    var potentialUsers = await _faceClient.Face.IdentifyAsync(faceGuids, PersonGroupId);
                    if (null != potentialUsers)
                    {
                        foreach (var candidate in potentialUsers.Select(identifyResult => identifyResult.Candidates.FirstOrDefault()))
                        {
                            recognizedCandidateList.Add(new RecognizedCandidate
                            {
                                Confidence = candidate.Confidence,
                                PersonId = $"{candidate.PersonId}"
                            });
                        }
                    }
                }
            }

            return recognizedCandidateList;
        }

        public async Task BeginTrainingAsync()
        {
            await EnsurePersonGroupCreated();

            await _faceClient.PersonGroup.TrainAsync(PersonGroupId);

        }

        public async Task<TrainingStatus> GetTrainingStatusAsync()
        {
            try
            {
                await EnsurePersonGroupCreated();

                _logger.LogDebug("Fetching Training Status from Azure Cognitive Services.");
                return await _faceClient.PersonGroup.GetTrainingStatusAsync(PersonGroupId);
            }
            catch (APIErrorException apiErrorException)
            {
                if (HttpStatusCode.NotFound == apiErrorException.Response.StatusCode)
                {
                    return new TrainingStatus();
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Models.Person> CreatePersonAsync(string name, string faceUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(faceUrl))
            {
                throw new ArgumentNullException(nameof(faceUrl));
            }

            await EnsurePersonGroupCreated();

            _logger.LogDebug("Creating person in Azure Cognitive Services.");
            var person = await _faceClient.PersonGroupPerson.CreateAsync(PersonGroupId, name);

            _logger.LogDebug("Adding face to newly created person in Azure Cognitive Services.");
            var persistedFace = await _faceClient.PersonGroupPerson.AddFaceFromUrlAsync(PersonGroupId, person.PersonId, faceUrl);
            if (null == persistedFace)
            {
                throw new InvalidOperationException("Failed to add face on newly created person.");
            }

            _logger.LogDebug("Starting training after adding person and face in Azure Cognitive Services.");
            //await _faceClient.PersonGroup.TrainAsync(PersonGroupId);

            return new Models.Person
            {
                PersonId = $"{person.PersonId}"
            };
        }

        public async Task DeletePersonAsync(string personId)
        {
            if (string.IsNullOrWhiteSpace(personId))
            {
                throw new ArgumentNullException(nameof(personId));
            }

            if (!Guid.TryParse(personId, out Guid personGuid))
            {
                throw new ArgumentException(nameof(personId));
            }

            await EnsurePersonGroupCreated();

            _logger.LogDebug("Deleting person in Azure Cognitive Services.");
            await _faceClient.PersonGroupPerson.DeleteAsync(PersonGroupId, personGuid);

            _logger.LogDebug("Starting training after removing person from Azure Cognitive Services.");
            // await _faceClient.PersonGroup.TrainAsync(PersonGroupId);
        }

        public async Task<Face> AddFaceAsync(string personId, string faceUrl)
        {
            if (string.IsNullOrWhiteSpace(personId))
            {
                throw new ArgumentNullException(nameof(personId));
            }

            if (string.IsNullOrWhiteSpace(faceUrl))
            {
                throw new ArgumentNullException(nameof(faceUrl));
            }

            if (!Guid.TryParse(personId, out Guid personGuid))
            {
                throw new ArgumentException(nameof(personId));
            }

            await EnsurePersonGroupCreated();

            _logger.LogDebug("Adding face to person in Azure Cognitive Services.");
            var persistedFace = await _faceClient.PersonGroupPerson.AddFaceFromUrlAsync(PersonGroupId, personGuid, faceUrl);
            if (null != persistedFace)
            {
                _logger.LogDebug("Starting training after removing person from Azure Cognitive Services.");
                //await _faceClient.PersonGroup.TrainAsync(PersonGroupId);

                return new Face
                {
                    FaceId = $"{persistedFace.PersistedFaceId}"
                };
            }

            throw new InvalidOperationException("Failed to add face to existing person.");
        }

        public async Task RemoveFaceAsync(string personId, string faceId)
        {
            if (string.IsNullOrWhiteSpace(personId))
            {
                throw new ArgumentNullException(nameof(personId));
            }

            if (string.IsNullOrWhiteSpace(faceId))
            {
                throw new ArgumentNullException(nameof(faceId));
            }

            if (!Guid.TryParse(personId, out Guid personGuid))
            {
                throw new ArgumentException(nameof(personId));
            }

            if (!Guid.TryParse(faceId, out Guid faceGuid))
            {
                throw new ArgumentException(nameof(faceId));
            }

            await EnsurePersonGroupCreated();

            _logger.LogDebug("Deleting face from person in Azure Cognitive Services.");
            await _faceClient.PersonGroupPerson.DeleteFaceAsync(PersonGroupId, personGuid, faceGuid);

            _logger.LogDebug("Starting training after removing face from Azure Cognitive Services.");
            //await _faceClient.PersonGroup.TrainAsync(PersonGroupId);
        }

        private async Task EnsurePersonGroupCreated()
        {
            try
            {
                await _faceClient.PersonGroup.GetAsync(PersonGroupId);
            }
            catch (APIErrorException apiErrorException)
            {
                if (HttpStatusCode.NotFound == apiErrorException.Response.StatusCode)
                {
                    await _faceClient.PersonGroup.CreateAsync(PersonGroupId, PersonGroupId, recognitionModel: _microserviceSettings.RecognitionModel);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
