using ContactlessEntry.Cloud.Models;
using System.Threading.Tasks;

namespace ContactlessEntry.Cloud.Services
{
    public interface IAccessManager
    {
        Task<Access> RequestAccessAsync(string doorId, string personId, double temperature);
    }
}
