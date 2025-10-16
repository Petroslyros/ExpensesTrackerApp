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

        }
    }
}
