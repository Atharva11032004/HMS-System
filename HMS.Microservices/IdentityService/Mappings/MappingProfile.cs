using AutoMapper;
using IdentityService.Models;

namespace IdentityService.Mappings;


// mapping profile in identity service . 
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>();
    }
}