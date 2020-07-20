using System.Threading.Tasks;

namespace ContactlessEntry.Cloud.Services
{
    public interface IOpenDoorService
    {
        Task OpenDoorAsync(string doorId, string personId);
    }
}
