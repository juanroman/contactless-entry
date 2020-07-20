using ContactlessEntry.Cloud.Models;
using System.Threading.Tasks;

namespace ContactlessEntry.Cloud.Services
{
    public interface IAccessRepository
    {
        Task<Access> CreateAccessAsync(Access access);
    }
}
