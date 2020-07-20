using AutoMapper;
using ContactlessEntry.Cloud.Models;
using ContactlessEntry.Cloud.Models.DataTransfer;

namespace ContactlessEntry.Cloud.Utilities
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RequestAccessDto, Access>();
            CreateMap<Access, AccessDto>();
        }
    }
}
