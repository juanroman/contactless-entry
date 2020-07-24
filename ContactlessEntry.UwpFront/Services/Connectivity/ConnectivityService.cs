using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ContactlessEntry.UwpFront.Services.Connectivity
{
    public class ConnectivityService : IConnectivityService
    {
        public async Task<bool> CheckIfConnectedToInternet()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("https://www.google.com");
                return response.IsSuccessStatusCode;
            }

            return false;
        }
    }
}
