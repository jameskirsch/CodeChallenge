using AutoMapper;
using CodeChallenge.Models;
using CodeChallenge.ViewModels;

namespace CodeChallenge.Config.MapperProfiles;

public class ReportingStructureProfile : Profile
{
    public ReportingStructureProfile()
    {
        CreateMap<ReportingStructure, ReportingStructureViewModel>()
            .ForMember(dest =>
                dest.Employee, opt => 
                opt.MapFrom(src => src.Employee))
            .ForMember(dest => 
                dest.NumberOfReports, opt => 
                opt.MapFrom(src => src.NumberOfReports));

        CreateMap<Employee, EmployeeViewModel>()
            .ForMember(dest => dest.DirectReports, opt => opt.MapFrom(src => src.DirectReports));
    }
}