using AutoMapper;
using StaffService.Models;

namespace StaffService.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Staff, StaffDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department!.Name));

        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.StaffCount, opt => opt.MapFrom(src => src.Staff.Count));

        CreateMap<CreateStaffRequest, Staff>();
        CreateMap<UpdateStaffRequest, Staff>();
        CreateMap<CreateDepartmentRequest, Department>();
    }
}