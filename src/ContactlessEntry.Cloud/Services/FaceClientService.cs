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
            IFaceClient faceClient,
            ILogger<FaceClientService> logger,
            IMicroserviceSettings microserviceSettings)
        {
            _faceClient = faceClient;
            _logger = logger;
            _microserviceSettings = microserviceSettings;
        }

        public Task<IList<RecognizedCandidate>> RecognizeWithStreamAsync(Stream image)
        {
            if (null == image)
            {
                throw new ArgumentNullException(nameof(image));
            }

            return RecognizeWithStreamImplementation(image);
        }

        private async Task<IList<RecognizedCandidate>> RecognizeWithStreamImplementation(Stream image)
        {
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

        public Task<Models.Person> CreatePersonAsync(string name, string faceUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(faceUrl))
            {
                throw new ArgumentNullException(nameof(faceUrl));
            }

            return CreatePersonAsyncImplementation(name, faceUrl);
        }

        private async Task<Models.Person> CreatePersonAsyncImplementation(string name, string faceUrl)
        {
            await EnsurePersonGroupCreated();

            _logger.LogDebug("Creating person in Azure Cognitive Services.");
            var person = await _faceClient.PersonGroupPerson.CreateAsync(PersonGroupId, name);

            _logger.LogDebug("Adding face to newly created person in Azure Cognitive Services.");
            await _faceClient.PersonGroupPerson.AddFaceFromUrlAsync(PersonGroupId, person.PersonId, faceUrl);

            return new Models.Person
            {
                PersonId = $"{person.PersonId}"
            };
        }

        public Task DeletePersonAsync(string personId)
        {
            if (string.IsNullOrWhiteSpace(personId))
            {
                throw new ArgumentNullException(nameof(personId));
            }

            if (!Guid.TryParse(personId, out Guid personGuid))
            {
                throw new ArgumentException(nameof(personId));
            }

            return DeletePersonAsyncImplementation(personGuid);
        }

        private async Task DeletePersonAsyncImplementation(Guid personGuid)
        {
            await EnsurePersonGroupCreated();

            _logger.LogDebug("Deleting person in Azure Cognitive Services.");
            await _faceClient.PersonGroupPerson.DeleteAsync(PersonGroupId, personGuid);
        }

        public Task<Face> AddFaceAsync(string personId, string faceUrl)
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

            return AddFaceAsyncImplementation(faceUrl, personGuid);
        }

        private async Task<Face> AddFaceAsyncImplementation(string faceUrl, Guid personGuid)
        {
            await EnsurePersonGroupCreated();

            _logger.LogDebug("Adding face to person in Azure Cognitive Services.");
            var persistedFace = await _faceClient.PersonGroupPerson.AddFaceFromUrlAsync(PersonGroupId, personGuid, faceUrl);

            return new Face
            {
                FaceId = $"{persistedFace?.PersistedFaceId}"
            };
        }

        public Task RemoveFaceAsync(string personId, string faceId)
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

            return RemoveFaceAsyncImplementation(personGuid, faceGuid);
        }

        private async Task RemoveFaceAsyncImplementation(Guid personGuid, Guid faceGuid)
        {
            await EnsurePersonGroupCreated();

            _logger.LogDebug("Deleting face from person in Azure Cognitive Services.");
            await _faceClient.PersonGroupPerson.DeleteFaceAsync(PersonGroupId, personGuid, faceGuid);
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
