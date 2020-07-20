using ContactlessEntry.Cloud.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ContactlessEntry.Cloud.Services
{
    public interface IFaceClientService
    {
        Task<IList<RecognizedCandidate>> RecognizeWithStreamAsync(Stream image);

        Task<Models.Person> CreatePersonAsync(string name, string faceUrl);

        Task DeletePersonAsync(string personId);

        Task<Face> AddFaceAsync(string personId, string faceUrl);

        Task RemoveFaceAsync(string personId, string faceId);

        Task<TrainingStatus> GetTrainingStatusAsync();
    }
}
