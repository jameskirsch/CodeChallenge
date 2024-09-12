
using CodeChallenge.Models;
using AutoMapper;

namespace CodeChallenge.Config.MapperProfiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, Employee>();
        }
    }
}
