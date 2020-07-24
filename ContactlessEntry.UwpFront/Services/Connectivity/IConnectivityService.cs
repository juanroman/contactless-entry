using System.Threading.Tasks;

namespace ContactlessEntry.UwpFront.Services.Connectivity
{
    public interface IConnectivityService
    {
        Task<bool> CheckIfConnectedToInternet();
    }
}
