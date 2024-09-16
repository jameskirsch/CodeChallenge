using CodeChallenge.Models;
using AutoMapper;
using CodeChallenge.ViewModels;

namespace CodeChallenge.Config.MapperProfiles;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, Employee>();
        CreateMap<Employee, EmployeeViewModel>()
            .ForMember(dest => dest.DirectReports, 
                opt => opt
                    .MapFrom(src => src.DirectReports));
    }
}