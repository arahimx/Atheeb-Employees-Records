using AutoMapper;
using Core.Entity;
using RestApi.ViewModels;
namespace WebApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserInfo, UsersListViewModel>();
            CreateMap<UserInfo, UsersListViewModel>().ReverseMap();
            CreateMap<UsersFormViewModel, UserInfo>()
                .ForMember(dest => dest!.FullName, opt => opt.MapFrom(src => src!.FullName))
                .ForMember(dest => dest!.FullName, opt => opt.MapFrom(src => src!.FullName))
                .ForMember(dest => dest!.FullName, opt => opt.MapFrom(src => src!.FullName))
                .ForMember(dest => dest!.FullName, opt => opt.MapFrom(src => src!.FullName));
        }
    }
}