using AutoMapper;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.DTO;

namespace ExpensesTrackerApp.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<User, UserReadOnlyDTO>().ReverseMap();
            // For registration map the properties that match
            CreateMap<UserRegisterDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.UserRole, opt => opt.Ignore());
            CreateMap<UpdateUserDTO, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            //The .ForAllMembers(...) condition ensures null fields won’t overwrite existing data if
            //they aren’t provided (useful for PATCH-like behavior).
        }
    }
}

